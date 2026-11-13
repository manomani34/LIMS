
using LIMS.Domain.Common;
using LIMS.Domain.Files.Enums;
using LIMS.Domain.Files.Events;

namespace LIMS.Domain.Files.Entities;

public sealed class FileReference : AggregateRoot<Guid>
{
    public Guid FileId { get; private set; }

    public FileReferenceType ReferenceType { get; private set; }

    public Guid ReferenceId { get; private set; }

    public string LinkedByUserId { get; private set; } = string.Empty;

    public string? Purpose { get; private set; }

    public DateTimeOffset LinkedAt { get; private set; }

    private bool _isRemoved;

    private FileReference()
    {
    }

    public FileReference(
        Guid fileId,
        FileReferenceType referenceType,
        Guid referenceId,
        string linkedByUserId,
        string? purpose,
        DateTimeOffset linkedAt)
    {
        if (fileId == Guid.Empty)
            throw new FileDomainException("FileId is required.");

        if (referenceId == Guid.Empty)
            throw new FileDomainException("ReferenceId is required.");

        if (!Enum.IsDefined(referenceType))
            throw new FileDomainException("Invalid file reference type.");

        if (string.IsNullOrWhiteSpace(linkedByUserId))
            throw new FileDomainException("LinkedByUserId is required.");

        var normalizedUserId = linkedByUserId.Trim();

        if (normalizedUserId.Length > 150)
            throw new FileDomainException(
                "LinkedByUserId cannot exceed 150 characters.");

        FileId = fileId;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
        LinkedByUserId = normalizedUserId;
        Purpose = NormalizePurpose(purpose);
        LinkedAt = linkedAt;

        Id = Guid.NewGuid();

        AddDomainEvent(
            new StoredFileReferenceAddedDomainEvent(
                FileId,
                ReferenceType,
                ReferenceId,
                linkedAt));
    }

    public void Unlink(DateTimeOffset unlinkedAt)
    {
        if (_isRemoved)
            return;

        _isRemoved = true;

        AddDomainEvent(
            new StoredFileReferenceRemovedDomainEvent(
                FileId,
                ReferenceType,
                ReferenceId,
                unlinkedAt));
    }

    private static string? NormalizePurpose(string? purpose)
    {
        if (string.IsNullOrWhiteSpace(purpose))
            return null;

        var normalized = purpose.Trim();

        if (normalized.Length > 500)
            throw new FileDomainException(
                "Purpose cannot exceed 500 characters.");

        return normalized;
    }
}
