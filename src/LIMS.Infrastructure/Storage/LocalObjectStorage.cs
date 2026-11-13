using LIMS.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace LIMS.Infrastructure.Storage;

public sealed class LocalObjectStorage : IObjectStorage
{
    private readonly string _rootPath;
    private readonly string _rootPathWithSeparator;

    public LocalObjectStorage(
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configuredPath =
            configuration["Storage:Local:RootPath"];

        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            configuredPath = "App_Data/LimsFiles";
        }

        _rootPath = Path.GetFullPath(
            Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(
                    environment.ContentRootPath,
                    configuredPath));

        _rootPathWithSeparator =
            _rootPath.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (!content.CanRead)
        {
            throw new ArgumentException(
                "The supplied stream is not readable.",
                nameof(content));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        var extension = GetSafeExtension(fileName);

        var datePath = DateTime.UtcNow.ToString("yyyy/MM");

        var fileNameOnStorage =
            $"{Guid.NewGuid():N}{extension}";

        var storageKey =
            $"{datePath}/{fileNameOnStorage}";

        var destinationPath = ResolvePath(storageKey);

        var directory = Path.GetDirectoryName(destinationPath)
            ?? throw new InvalidOperationException(
                "Unable to determine the storage directory.");

        Directory.CreateDirectory(directory);

        try
        {
            await using var destination = new FileStream(
                destinationPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                options: FileOptions.Asynchronous |
                         FileOptions.SequentialScan);

            await content.CopyToAsync(
                destination,
                cancellationToken);

            await destination.FlushAsync(cancellationToken);

            return storageKey;
        }
        catch
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            throw;
        }
    }

    public Task<Stream?> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var filePath = ResolvePath(storageKey);

        if (!File.Exists(filePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            options: FileOptions.Asynchronous |
                     FileOptions.SequentialScan);

        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var filePath = ResolvePath(storageKey);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    private string ResolvePath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));
        }

        if (Path.IsPathRooted(storageKey))
        {
            throw new ArgumentException(
                "Rooted storage keys are not allowed.",
                nameof(storageKey));
        }

        var normalizedKey = storageKey
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        var segments = normalizedKey.Split(
            Path.DirectorySeparatorChar,
            StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length == 0 ||
            segments.Any(segment =>
                segment is "." or ".." ||
                segment.Contains(':')))
        {
            throw new ArgumentException(
                "Invalid storage key.",
                nameof(storageKey));
        }

        var fullPath = Path.GetFullPath(
            Path.Combine(_rootPath, normalizedKey));

        if (!fullPath.StartsWith(
                _rootPathWithSeparator,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Storage key points outside the storage directory.",
                nameof(storageKey));
        }

        return fullPath;
    }

    private static string GetSafeExtension(string fileName)
    {
        var extension = Path.GetExtension(
            Path.GetFileName(fileName));

        if (string.IsNullOrWhiteSpace(extension) ||
            extension.Length > 20)
        {
            return string.Empty;
        }

        var isSafe = extension
            .Skip(1)
            .All(character =>
                char.IsAsciiLetterOrDigit(character));

        return isSafe
            ? extension.ToLowerInvariant()
            : string.Empty;
    }
}
