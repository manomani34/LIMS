
namespace LIMS.Contracts.Samples.Responses;

public sealed record SampleResponse(
    Guid Id,
    Guid LaboratoryId,
    string SampleCode,
    string Name,
    string? Description,
    string? RejectionReason,
    string Type,
    string Status,
    string Priority,
    string? Source,
    string? AssignedToUserId,
    string? RegisteredByUserId,
    DateTimeOffset RegisteredAt,
    DateTimeOffset? ReceivedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? ArchivedAt);
