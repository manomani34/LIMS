using LIMS.Application.Files.DTOs;
using LIMS.Contracts.Files;
using LIMS.Contracts.Files.Responses;
using LIMS.Domain.Files.Enums;

namespace LIMS.Api.Areas.Files.Mappings;

public static class FileContractMappings
{
    public static FileCategory ToDomain(this FileCategoryContract value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid file category.");
        }

        return (FileCategory)(int)value;
    }

    public static FileReferenceType ToDomain(
        this FileReferenceTypeContract value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid file reference type.");
        }

        return (FileReferenceType)(int)value;
    }

    public static FileResponse ToResponse(this FileAssetDto dto)
    {
        return new FileResponse(
            dto.Id,
            dto.OriginalFileName,
            dto.ContentType,
            dto.FileExtension,
            dto.SizeInBytes,
            dto.Sha256Checksum,
            dto.Category.ToString(),
            dto.Status.ToString(),
            dto.UploadedByUserId,
            dto.Description,
            dto.QuarantineReason,
            dto.RejectionReason,
            dto.UploadedAt,
            dto.AvailableAt,
            dto.QuarantinedAt,
            dto.DeletedAt);
    }

    public static FileReferenceResponse ToResponse(
        this FileReferenceDto dto)
    {
        return new FileReferenceResponse(
            dto.Id,
            dto.FileId,
            dto.ReferenceType.ToString(),
            dto.ReferenceId,
            dto.LinkedByUserId,
            dto.Purpose,
            dto.LinkedAt);
    }
}
