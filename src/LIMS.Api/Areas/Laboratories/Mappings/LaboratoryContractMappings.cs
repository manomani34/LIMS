
using LIMS.Application.Laboratories.DTOs;
using LIMS.Contracts.Laboratories;
using LIMS.Contracts.Laboratories.Responses;
using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Api.Areas.Laboratories.Mappings;

public static class LaboratoryContractMappings
{
    public static LaboratoryMemberRole ToDomain(
        this LaboratoryMemberRoleContract value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid laboratory member role.");
        }

        return (LaboratoryMemberRole)(int)value;
    }

    public static LaboratoryStatus ToDomain(
        this LaboratoryStatusContract value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid laboratory status.");
        }

        return (LaboratoryStatus)(int)value;
    }

    public static LaboratoryResponse ToResponse(this LaboratoryDto dto)
    {
        return new LaboratoryResponse(
            dto.Id,
            dto.Name,
            dto.Code,
            dto.Description,
            dto.Status.ToString(),
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.MemberCount);
    }

    public static LaboratoryDetailsResponse ToDetailsResponse(
        this LaboratoryDetailsDto dto)
    {
        return new LaboratoryDetailsResponse(
            dto.Id,
            dto.Name,
            dto.Code,
            dto.Description,
            dto.Status.ToString(),
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.Members
                .Select(member => new LaboratoryMemberResponse(
                    member.Id,
                    member.UserId,
                    member.Role.ToString(),
                    member.JoinedAt))
                .ToArray());
    }
}
