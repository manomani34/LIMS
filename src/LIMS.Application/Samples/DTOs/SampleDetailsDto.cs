using LIMS.Domain.Samples.Enums;

namespace LIMS.Application.Samples.DTOs;

public sealed record SampleDetailsDto(
    Guid Id,
    Guid LaboratoryId,
    string SampleCode,
    string Name,
    string? Description,
    SampleType Type,
    SampleStatus Status,
    SamplePriority Priority,
    string? Source,
    string? AssignedToUserId,
    string RegisteredByUserId,
    DateTimeOffset RegisteredAt,
    DateTimeOffset? ReceivedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? ArchivedAt);
