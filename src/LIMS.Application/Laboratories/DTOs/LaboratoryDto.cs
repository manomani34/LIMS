using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Application.Laboratories.DTOs;

public sealed record LaboratoryDto(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    LaboratoryStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int MemberCount);
