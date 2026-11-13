namespace LIMS.Contracts.Protocols;

public sealed record ProtocolResponse(
    Guid Id,
    Guid LaboratoryId,
    string ProtocolCode,
    string Name,
    string? Description,
    string? Version,
    ProtocolStatusContract Status,
    string CreatedByUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ActivatedAt,
    DateTimeOffset? DeprecatedAt,
    DateTimeOffset? ArchivedAt);
