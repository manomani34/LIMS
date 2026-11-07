using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Application.Laboratories.DTOs;

public sealed record LaboratoryMemberDto(
    Guid Id,
    string UserId,
    LaboratoryMemberRole Role,
    DateTimeOffset JoinedAt);
