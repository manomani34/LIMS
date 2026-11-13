
using LIMS.Domain.Common;
using LIMS.Domain.Files.Enums;
using LIMS.Domain.Files.Events;
using LIMS.Domain.Files.ValueObjects;

namespace LIMS.Domain.Files.Entities;

public sealed class FileAsset : AggregateRoot<Guid>
{
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string FileExtension { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public string Sha256Checksum { get; private set; } = string.Empty;

    public FileCategory Category { get; private set; }

    public FileStatus Status { get; private set; }

    public string UploadedByUserId { get; private set; } = string.Empty;

    public string? Description { get; private set; }
    public string? QuarantineReason { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTimeOffset UploadedAt { get; private set; }
    public DateTimeOffset? AvailableAt { get; private set; }
    public DateTimeOffset? QuarantinedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    private FileAsset()
    {
    }

    public static FileAsset Register(
        string originalFileName,
        string storageKey,
        string contentType,
        string fileExtension,
        long sizeInBytes,
        string sha256Checksum,
        FileCategory category,
        string uploadedByUserId,
        string? description,
        DateTimeOffset uploadedAt)
    {
        if (!Enum.IsDefined(category))
            throw new FileDomainException("Invalid file category.");

        if (sizeInBytes <= 0)
            throw new FileDomainException("File size must be greater than zero.");

        var checksum = FileChecksum.CreateSha256(sha256Checksum);

        var file = new FileAsset
        {
            Id = Guid.NewGuid(),
            OriginalFileName = NormalizeRequired(
                originalFileName,
                nameof(originalFileName),
                255),
            StorageKey = NormalizeRequired(
                storageKey,
                nameof(storageKey),
                1000),
            ContentType = NormalizeRequired(
                contentType,
                nameof(contentType),
                200),
            FileExtension = NormalizeRequired(
                fileExtension,
                nameof(fileExtension),
                20),
            SizeInBytes = sizeInBytes,
            Sha256Checksum = checksum.Value,
            Category = category,
            UploadedByUserId = NormalizeRequired(
                uploadedByUserId,
                nameof(uploadedByUserId),
                150),
            Description = NormalizeOptional(description, 2000),
            Status = FileStatus.PendingUpload,
            UploadedAt = uploadedAt
        };

        file.AddDomainEvent(
            new StoredFileRegisteredDomainEvent(
                file.Id,
                file.OriginalFileName,
                uploadedAt));

        return file;
    }

    public void MarkAvailable(DateTimeOffset availableAt)
    {
        if (Status != FileStatus.PendingUpload)
        {
            throw new FileDomainException(
                "Only files pending upload can be marked as available.");
        }

        Status = FileStatus.Available;
        AvailableAt = availableAt;

        AddDomainEvent(
            new StoredFileUploadedDomainEvent(
                Id,
                availableAt));
    }

    public void Quarantine(
        string reason,
        DateTimeOffset quarantinedAt)
    {
        if (Status != FileStatus.Available)
        {
            throw new FileDomainException(
                "Only available files can be quarantined.");
        }

        QuarantineReason = NormalizeRequired(
            reason,
            nameof(reason),
            1000);

        Status = FileStatus.Quarantined;
        QuarantinedAt = quarantinedAt;
    }

    public void ReleaseFromQuarantine(DateTimeOffset releasedAt)
    {
        if (Status != FileStatus.Quarantined)
        {
            throw new FileDomainException(
                "Only quarantined files can be released.");
        }

        Status = FileStatus.Available;
        QuarantineReason = null;
        QuarantinedAt = null;
        AvailableAt = releasedAt;
    }

    public void Reject(string reason)
    {
        if (Status is FileStatus.Deleted or FileStatus.Rejected)
        {
            throw new FileDomainException(
                "This file cannot be rejected in its current status.");
        }

        RejectionReason = NormalizeRequired(
            reason,
            nameof(reason),
            1000);

        Status = FileStatus.Rejected;
    }

    public void Delete(
        string deletedByUserId,
        DateTimeOffset deletedAt)
    {
        if (Status == FileStatus.Deleted)
            return;

        _ = NormalizeRequired(
            deletedByUserId,
            nameof(deletedByUserId),
            150);

        Status = FileStatus.Deleted;
        DeletedAt = deletedAt;

        AddDomainEvent(
            new StoredFileDeletedDomainEvent(
                Id,
                deletedAt));
    }

    public void UpdateDescription(string? description)
    {
        if (Status is FileStatus.Deleted or FileStatus.Rejected)
        {
            throw new FileDomainException(
                "Description cannot be updated for deleted or rejected files.");
        }

        Description = NormalizeOptional(description, 2000);
    }

    private static string NormalizeRequired(
        string? value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FileDomainException(
                $"{fieldName} is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new FileDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new FileDomainException(
                $"Value cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}
