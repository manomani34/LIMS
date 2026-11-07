namespace LIMS.Contracts.Protocols;

public sealed record ProtocolDetailsResponse(
    Guid Id,
    Guid LaboratoryId,
    string ProtocolCode,
    string Name,
    string? Description,
    string Version,
    ProtocolStatusContract Status,
    string CreatedByUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ActivatedAt,
    DateTimeOffset? DeprecatedAt,
    DateTimeOffset? ArchivedAt,
    IReadOnlyCollection<ProtocolStepResponse> Steps);
