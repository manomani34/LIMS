using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.DTOs;

public sealed record AnalysisResultDto(
    Guid Id,
    Guid LaboratoryId,
    Guid ExperimentId,
    Guid SampleId,
    Guid? InstrumentId,
    string ResultCode,
    string Title,
    string? Description,
    ResultStatus Status,
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
