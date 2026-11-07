using LIMS.Application.Requests.DTOs;
using LIMS.Domain.Requests;

namespace LIMS.Application.Requests.Mapping;

public static class SampleRequestMapping
{
    public static SampleRequestDto ToDto(this SampleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new SampleRequestDto(
            request.Id,
            request.RequestCode,
            request.Title,
            request.Description,
            request.SampleName,
            request.SampleDescription,
            request.RequestedSampleType,
            request.SampleSource,
            request.RequestedByUserId,
            request.AssignedReviewerUserId,
            request.LaboratoryId,
            request.CreatedSampleId,
            request.Priority,
            request.Status,
            request.ReviewComments,
            request.RejectionReason,
            request.CancellationReason,
            request.CreatedAt,
            request.UpdatedAt,
            request.SubmittedAt,
            request.ReviewStartedAt,
            request.ApprovedAt,
            request.RejectedAt,
            request.CancelledAt,
            request.FulfilledAt);
    }
}
