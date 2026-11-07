using LIMS.Domain.Protocols.Enums;

namespace LIMS.Application.Protocols.DTOs;

public sealed record ProtocolDetailsDto(
    Guid Id,
    Guid LaboratoryId,
    string ProtocolCode,
    string Name,
    string? Description,
    string? Version,
    ProtocolStatus Status,
    string CreatedByUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ActivatedAt,
    DateTimeOffset? DeprecatedAt,
    DateTimeOffset? ArchivedAt,
    IReadOnlyCollection<ProtocolStepDto> Steps);
