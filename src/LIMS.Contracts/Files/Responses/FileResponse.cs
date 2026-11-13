namespace LIMS.Contracts.Files.Responses;

public sealed record FileResponse(
    Guid Id,
    string OriginalFileName,
    string ContentType,
    string FileExtension,
    long SizeInBytes,
    string Sha256Checksum,
    string Category,
    string Status,
    string UploadedByUserId,
    string? Description,
    string? QuarantineReason,
    string? RejectionReason,
    DateTime UploadedAt,
    DateTime? AvailableAt,
    DateTime? QuarantinedAt,
    DateTime? DeletedAt);
