using LIMS.Application.Files.DTOs;
using LIMS.Domain.Files;
using LIMS.Domain.Files.Entities;

namespace LIMS.Application.Files.Mapping;

public static class FileMapping
{
    public static FileAssetDto ToDto(this FileAsset file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return new FileAssetDto(
            file.Id,
            file.OriginalFileName,
            file.ContentType,
            file.FileExtension,
            file.SizeInBytes,
            file.Sha256Checksum,
            file.Category,
            file.Status,
            file.UploadedByUserId,
            file.Description,
            file.QuarantineReason,
            file.RejectionReason,
            file.UploadedAt,
            file.AvailableAt,
            file.QuarantinedAt,
            file.DeletedAt);
    }

    public static FileReferenceDto ToDto(this FileReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return new FileReferenceDto(
            reference.Id,
            reference.FileId,
            reference.ReferenceType,
            reference.ReferenceId,
            reference.LinkedByUserId,
            reference.Purpose,
            reference.LinkedAt);
    }
}
