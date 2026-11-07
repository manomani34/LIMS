using LIMS.Application.Requests.DTOs;
using LIMS.Contracts.Requests.Enums;
using LIMS.Contracts.Requests.Responses;
using LIMS.Contracts.Samples;
using LIMS.Domain.Requests.Enums;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Contracts.Requests.Mapping;

public static class SampleRequestContractMappings
{
    public static SampleRequestResponse ToResponse(
        this SampleRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new SampleRequestResponse(
            request.Id,
            request.RequestCode,
            request.Title,
            request.Description,
            request.SampleName,
            request.SampleDescription,
            request.RequestedSampleType.ToContract(),
            request.SampleSource,
            request.RequestedByUserId,
            request.AssignedReviewerUserId,
            request.LaboratoryId,
            request.CreatedSampleId,
            request.Priority.ToContract(),
            request.Status.ToContract(),
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

    public static SampleRequestStatusContract ToContract(
        this SampleRequestStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return (SampleRequestStatusContract)(int)status;
    }

    public static SampleRequestPriorityContract ToContract(
        this SampleRequestPriority priority)
    {
        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        return (SampleRequestPriorityContract)(int)priority;
    }

    public static SampleType ToDomain(
        this SampleTypeContract sampleType)
    {
        if (!Enum.IsDefined(sampleType))
            throw new ArgumentOutOfRangeException(nameof(sampleType));

        return (SampleType)(int)sampleType;
    }

    public static SampleRequestPriority ToDomain(
        this SampleRequestPriorityContract priority)
    {
        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        return (SampleRequestPriority)(int)priority;
    }
}
