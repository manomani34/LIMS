namespace LIMS.Contracts.Experiments.Responses;

public sealed record ExperimentDetailsResponse(
    Guid Id,
    Guid LaboratoryId,
    Guid SampleId,
    string ExperimentCode,
    string Title,
    string? Description,
    string Status,
    string CreatedByUserId,
    string? AssignedToUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? ArchivedAt);
