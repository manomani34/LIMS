using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.DTOs;

public sealed record FileReferenceDto(
    Guid Id,
    Guid FileId,
    FileReferenceType ReferenceType,
    Guid ReferenceId,
    string LinkedByUserId,
    string? Purpose,
    DateTime LinkedAt);
