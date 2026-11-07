using LIMS.Domain.Requests.Enums;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Application.Requests.DTOs;

public sealed record SampleRequestDto(
    Guid Id,
    string RequestCode,
    string Title,
    string? Description,
    string SampleName,
    string? SampleDescription,
    SampleType RequestedSampleType,
    string? SampleSource,
    string RequestedByUserId,
    string? AssignedReviewerUserId,
    Guid? LaboratoryId,
    Guid? CreatedSampleId,
    SampleRequestPriority Priority,
    SampleRequestStatus Status,
    string? ReviewComments,
    string? RejectionReason,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? SubmittedAt,
    DateTime? ReviewStartedAt,
    DateTime? ApprovedAt,
    DateTime? RejectedAt,
    DateTime? CancelledAt,
    DateTime? FulfilledAt);
