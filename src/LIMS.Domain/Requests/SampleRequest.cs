using LIMS.Domain.Common;
using LIMS.Domain.Requests.Enums;
using LIMS.Domain.Requests.Events;
using LIMS.Domain.Requests.Exceptions;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Domain.Requests;

public sealed class SampleRequest : AggregateRoot<Guid>
{
    public string RequestCode { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string SampleName { get; private set; } = string.Empty;

    public string? SampleDescription { get; private set; }

    public SampleType RequestedSampleType { get; private set; }

    public string? SampleSource { get; private set; }

    public string RequestedByUserId { get; private set; } = string.Empty;

    public string? AssignedReviewerUserId { get; private set; }

    public Guid? LaboratoryId { get; private set; }

    public Guid? CreatedSampleId { get; private set; }

    public SampleRequestPriority Priority { get; private set; }

    public SampleRequestStatus Status { get; private set; }

    public string? ReviewComments { get; private set; }

    public string? RejectionReason { get; private set; }

    public string? CancellationReason { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? SubmittedAt { get; private set; }

    public DateTime? ReviewStartedAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    public DateTime? RejectedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public DateTime? FulfilledAt { get; private set; }

    private SampleRequest()
    {
    }

    private SampleRequest(
        Guid id,
        string requestCode,
        string title,
        string? description,
        string sampleName,
        string? sampleDescription,
        SampleType requestedSampleType,
        string? sampleSource,
        string requestedByUserId,
        SampleRequestPriority priority,
        DateTime createdAt)
        : base(id)
    {
        RequestCode = NormalizeRequired(requestCode, nameof(requestCode), 50)
            .ToUpperInvariant();

        ValidateCode(RequestCode);

        Title = NormalizeRequired(title, nameof(title), 200);
        Description = NormalizeOptional(description, 4000);
        SampleName = NormalizeRequired(sampleName, nameof(sampleName), 200);
        SampleDescription = NormalizeOptional(sampleDescription, 2000);
        SampleSource = NormalizeOptional(sampleSource, 250);
        RequestedByUserId = NormalizeRequired(
            requestedByUserId,
            nameof(requestedByUserId),
            150);

        if (!Enum.IsDefined(requestedSampleType))
            throw new SampleRequestDomainException("Requested sample type is invalid.");

        if (!Enum.IsDefined(priority))
            throw new SampleRequestDomainException("Request priority is invalid.");

        RequestedSampleType = requestedSampleType;
        Priority = priority;
        Status = SampleRequestStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public static SampleRequest Create(
        string requestCode,
        string title,
        string? description,
        string sampleName,
        string? sampleDescription,
        SampleType requestedSampleType,
        string? sampleSource,
        string requestedByUserId,
        SampleRequestPriority priority,
        DateTime createdAt)
    {
        var request = new SampleRequest(
            Guid.NewGuid(),
            requestCode,
            title,
            description,
            sampleName,
            sampleDescription,
            requestedSampleType,
            sampleSource,
            requestedByUserId,
            priority,
            createdAt);

        request.AddDomainEvent(
            new SampleRequestCreatedDomainEvent(
                request.Id,
                request.RequestCode,
                request.RequestedByUserId,
                createdAt));

        return request;
    }

    public void UpdateInformation(
        string title,
        string? description,
        string sampleName,
        string? sampleDescription,
        SampleType requestedSampleType,
        string? sampleSource,
        SampleRequestPriority priority,
        DateTime updatedAt)
    {
        EnsureDraft();

        Title = NormalizeRequired(title, nameof(title), 200);
        Description = NormalizeOptional(description, 4000);
        SampleName = NormalizeRequired(sampleName, nameof(sampleName), 200);
        SampleDescription = NormalizeOptional(sampleDescription, 2000);
        SampleSource = NormalizeOptional(sampleSource, 250);

        if (!Enum.IsDefined(requestedSampleType))
            throw new SampleRequestDomainException("Requested sample type is invalid.");

        if (!Enum.IsDefined(priority))
            throw new SampleRequestDomainException("Request priority is invalid.");

        RequestedSampleType = requestedSampleType;
        Priority = priority;
        UpdatedAt = updatedAt;
    }

    public void Submit(
        string submittedByUserId,
        DateTime submittedAt)
    {
        EnsureDraft();

        var userId = NormalizeRequired(
            submittedByUserId,
            nameof(submittedByUserId),
            150);

        if (!string.Equals(
                userId,
                RequestedByUserId,
                StringComparison.Ordinal))
        {
            throw new SampleRequestDomainException(
                "Only the request creator can submit this request.");
        }

        Status = SampleRequestStatus.Submitted;
        SubmittedAt = submittedAt;
        UpdatedAt = submittedAt;

        AddDomainEvent(
            new SampleRequestSubmittedDomainEvent(
                Id,
                userId,
                submittedAt,
                submittedAt));
    }

    public void StartReview(
        string reviewerUserId,
        DateTime reviewedAt)
    {
        EnsureStatus(SampleRequestStatus.Submitted);

        var reviewer = NormalizeRequired(
            reviewerUserId,
            nameof(reviewerUserId),
            150);

        if (string.Equals(
                reviewer,
                RequestedByUserId,
                StringComparison.Ordinal))
        {
            throw new SampleRequestDomainException(
                "The request creator cannot review their own request.");
        }

        AssignedReviewerUserId = reviewer;
        ReviewStartedAt = reviewedAt;
        Status = SampleRequestStatus.UnderReview;
        UpdatedAt = reviewedAt;

        AddDomainEvent(
            new SampleRequestReviewStartedDomainEvent(
                Id,
                reviewer,
                reviewedAt,
                reviewedAt));
    }

    public void Approve(
        string approvedByUserId,
        Guid laboratoryId,
        string? comments,
        DateTime approvedAt)
    {
        EnsureStatus(SampleRequestStatus.UnderReview);

        var approver = NormalizeRequired(
            approvedByUserId,
            nameof(approvedByUserId),
            150);

        EnsureAssignedReviewer(approver);

        if (laboratoryId == Guid.Empty)
            throw new SampleRequestDomainException("Laboratory ID is required.");

        LaboratoryId = laboratoryId;
        ReviewComments = NormalizeOptional(comments, 2000);
        ApprovedAt = approvedAt;
        Status = SampleRequestStatus.Approved;
        UpdatedAt = approvedAt;

        AddDomainEvent(
            new SampleRequestApprovedDomainEvent(
                Id,
                approver,
                laboratoryId,
                approvedAt,
                approvedAt));
    }

    public void Reject(
        string rejectedByUserId,
        string reason,
        DateTime rejectedAt)
    {
        EnsureStatus(SampleRequestStatus.UnderReview);

        var reviewer = NormalizeRequired(
            rejectedByUserId,
            nameof(rejectedByUserId),
            150);

        EnsureAssignedReviewer(reviewer);

        RejectionReason = NormalizeRequired(reason, nameof(reason), 2000);
        RejectedAt = rejectedAt;
        Status = SampleRequestStatus.Rejected;
        UpdatedAt = rejectedAt;

        AddDomainEvent(
            new SampleRequestRejectedDomainEvent(
                Id,
                reviewer,
                RejectionReason,
                rejectedAt,
                rejectedAt));
    }

    public void Cancel(
        string cancelledByUserId,
        string? reason,
        DateTime cancelledAt)
    {
        var userId = NormalizeRequired(
            cancelledByUserId,
            nameof(cancelledByUserId),
            150);

        if (Status is SampleRequestStatus.Cancelled
            or SampleRequestStatus.Fulfilled)
        {
            throw new SampleRequestDomainException(
                "A cancelled or fulfilled request cannot be cancelled.");
        }

        if (Status == SampleRequestStatus.Approved)
        {
            throw new SampleRequestDomainException(
                "An approved request cannot be cancelled after laboratory assignment.");
        }

        if (!string.Equals(
                userId,
                RequestedByUserId,
                StringComparison.Ordinal))
        {
            throw new SampleRequestDomainException(
                "Only the request creator can cancel this request.");
        }

        CancellationReason = NormalizeOptional(reason, 2000);
        CancelledAt = cancelledAt;
        Status = SampleRequestStatus.Cancelled;
        UpdatedAt = cancelledAt;

        AddDomainEvent(
            new SampleRequestCancelledDomainEvent(
                Id,
                userId,
                CancellationReason,
                cancelledAt,
                cancelledAt));
    }

    public void Fulfill(
        Guid sampleId,
        DateTime fulfilledAt)
    {
        EnsureStatus(SampleRequestStatus.Approved);

        if (sampleId == Guid.Empty)
            throw new SampleRequestDomainException("Sample ID is required.");

        CreatedSampleId = sampleId;
        FulfilledAt = fulfilledAt;
        Status = SampleRequestStatus.Fulfilled;
        UpdatedAt = fulfilledAt;

        AddDomainEvent(
            new SampleRequestFulfilledDomainEvent(
                Id,
                sampleId,
                fulfilledAt,
                fulfilledAt));
    }

    private void EnsureDraft()
    {
        EnsureStatus(SampleRequestStatus.Draft);
    }

    private void EnsureStatus(SampleRequestStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new SampleRequestDomainException(
                $"Operation requires request status {expectedStatus}. Current status is {Status}.");
        }
    }

    private void EnsureAssignedReviewer(string userId)
    {
        if (!string.Equals(
                AssignedReviewerUserId,
                userId,
                StringComparison.Ordinal))
        {
            throw new SampleRequestDomainException(
                "Only the assigned reviewer can perform this operation.");
        }
    }

    private static string NormalizeRequired(
        string? value,
        string fieldName,
        int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new SampleRequestDomainException($"{fieldName} is required.");

        if (normalized.Length > maxLength)
        {
            throw new SampleRequestDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
        {
            throw new SampleRequestDomainException(
                $"Value cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static void ValidateCode(string code)
    {
        foreach (var character in code)
        {
            if (!char.IsAsciiLetterOrDigit(character)
                && character != '-'
                && character != '_')
            {
                throw new SampleRequestDomainException(
                    "Request code can contain only letters, numbers, hyphens and underscores.");
            }
        }
    }
}
