using LIMS.Contracts.Results.Enums;

namespace LIMS.Contracts.Results.Responses;

public sealed record AnalysisResultResponse(
    Guid Id,
    Guid LaboratoryId,
    Guid ExperimentId,
    Guid SampleId,
    Guid? InstrumentId,
    string ResultCode,
    string Title,
    string? Description,
    ResultStatusContract Status,
    string CreatedByUserId,
    string? SubmittedByUserId,
    string? ReviewedByUserId,
    string? ReviewComments,
    string? RejectionReason,
    string? ArchivedByUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? ReviewedAt,
    DateTimeOffset? ArchivedAt);
