using LIMS.Domain.Experiments.Enums;

namespace LIMS.Application.Experiments.DTOs;

public sealed record ExperimentDetailsDto(
    Guid Id,
    Guid LaboratoryId,
    Guid SampleId,
    string ExperimentCode,
    string Title,
    string? Description,
    ExperimentStatus Status,
    string CreatedByUserId,
    string? AssignedToUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? ArchivedAt);
