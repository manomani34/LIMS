using LIMS.Contracts.Requests.Enums;
using LIMS.Contracts.Samples;

namespace LIMS.Contracts.Requests.Responses;

public sealed record SampleRequestResponse(
    Guid Id,
    string RequestCode,
    string Title,
    string? Description,
    string SampleName,
    string? SampleDescription,
    SampleTypeContract RequestedSampleType,
    string? SampleSource,
    string RequestedByUserId,
    string? AssignedReviewerUserId,
    Guid? LaboratoryId,
    Guid? CreatedSampleId,
    SampleRequestPriorityContract Priority,
    SampleRequestStatusContract Status,
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
