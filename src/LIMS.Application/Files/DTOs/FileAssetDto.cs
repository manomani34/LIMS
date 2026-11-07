using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.DTOs;

public sealed record FileAssetDto(
    Guid Id,
    string OriginalFileName,
    string ContentType,
    string FileExtension,
    long SizeInBytes,
    string Sha256Checksum,
    FileCategory Category,
    FileStatus Status,
    string UploadedByUserId,
    string? Description,
    string? QuarantineReason,
    string? RejectionReason,
    DateTime UploadedAt,
    DateTime? AvailableAt,
    DateTime? QuarantinedAt,
    DateTime? DeletedAt);
