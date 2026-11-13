using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Storage;
using LIMS.Application.Files.DTOs;
using LIMS.Application.Files.Queries;
using LIMS.Application.Files.Repositories;
using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.Handlers;

public sealed class DownloadFileQueryHandler
    : IQueryHandler<DownloadFileQuery, FileDownloadDto?>
{
    private readonly IFileAssetRepository _repository;
    private readonly IObjectStorage _objectStorage;

    public DownloadFileQueryHandler(
        IFileAssetRepository repository,
        IObjectStorage objectStorage)
    {
        _repository = repository;
        _objectStorage = objectStorage;
    }

    public async Task<FileDownloadDto?> HandleAsync(
        DownloadFileQuery query,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            query.FileId,
            cancellationToken);

        if (file is null || file.Status != FileStatus.Available)
        {
            return null;
        }

        var stream = await _objectStorage.DownloadAsync(
            file.StorageKey,
            cancellationToken);

        if (stream is null)
        {
            return null;
        }

        return new FileDownloadDto(
            stream,
            file.OriginalFileName,
            file.ContentType,
            file.SizeInBytes);
    }
}
