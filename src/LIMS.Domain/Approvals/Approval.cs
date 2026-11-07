using LIMS.Domain.Common;
using LIMS.Domain.Approvals.Entities;
using LIMS.Domain.Approvals.Enums;
using LIMS.Domain.Approvals.Events;
using LIMS.Domain.Approvals.Exceptions;

namespace LIMS.Domain.Approvals;

public sealed class Approval : AggregateRoot<Guid>
{
    private readonly List<ApprovalReviewer> _reviewers = [];
    private readonly List<ApprovalDecisionRecord> _decisions = [];

    public ApprovalSubjectType SubjectType { get; private set; }

    public Guid SubjectId { get; private set; }

    public string RequestedByUserId { get; private set; } = string.Empty;

    public string? Title { get; private set; }

    public string? Description { get; private set; }

    public ApprovalStatus Status { get; private set; }

    public int RequiredApprovalCount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ReviewStartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<ApprovalReviewer> Reviewers =>
        _reviewers.AsReadOnly();

    public IReadOnlyCollection<ApprovalDecisionRecord> Decisions =>
        _decisions.AsReadOnly();

    private Approval()
    {
    }

    private Approval(
        Guid id,
        ApprovalSubjectType subjectType,
        Guid subjectId,
        string requestedByUserId,
        string? title,
        string? description,
        int requiredApprovalCount,
        DateTime createdAt)
        : base(id)
    {
        if (!Enum.IsDefined(subjectType))
            throw new ApprovalDomainException("Approval subject type is invalid.");

        if (subjectId == Guid.Empty)
            throw new ApprovalDomainException("Approval subject ID is required.");

        if (requiredApprovalCount <= 0)
            throw new ApprovalDomainException(
                "Required approval count must be greater than zero.");

        SubjectType = subjectType;
        SubjectId = subjectId;
        RequestedByUserId = NormalizeRequired(
            requestedByUserId,
            nameof(requestedByUserId),
            150);

        Title = NormalizeOptional(title, 200);
        Description = NormalizeOptional(description, 4000);
        RequiredApprovalCount = requiredApprovalCount;
        Status = ApprovalStatus.Pending;
        CreatedAt = createdAt;
    }

    public static Approval Create(
        ApprovalSubjectType subjectType,
        Guid subjectId,
        string requestedByUserId,
        string? title,
        string? description,
        int requiredApprovalCount,
        DateTime createdAt)
    {
        var approval = new Approval(
            Guid.NewGuid(),
            subjectType,
            subjectId,
            requestedByUserId,
            title,
            description,
            requiredApprovalCount,
            createdAt);

        approval.AddDomainEvent(
            new ApprovalCreatedDomainEvent(
                approval.Id,
                subjectType,
                subjectId,
                approval.RequestedByUserId,
                createdAt));

        return approval;
    }

    public Guid AddReviewer(
        string reviewerUserId,
        int sequence,
        bool isRequired,
        DateTime assignedAt)
    {
        EnsurePending();

        var normalizedUserId = NormalizeRequired(
            reviewerUserId,
            nameof(reviewerUserId),
            150);

        if (string.Equals(
                normalizedUserId,
                RequestedByUserId,
                StringComparison.Ordinal))
        {
            throw new ApprovalDomainException(
                "The approval requester cannot review their own approval.");
        }

        if (_reviewers.Any(x => string.Equals(
                x.UserId,
                normalizedUserId,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new ApprovalDomainException(
                "The reviewer is already assigned to this approval.");
        }

        if (_reviewers.Any(x => x.Sequence == sequence))
            throw new ApprovalDomainException(
                "A reviewer with this sequence already exists.");

        var reviewer = new ApprovalReviewer(
            Id,
            normalizedUserId,
            sequence,
            isRequired,
            assignedAt);

        _reviewers.Add(reviewer);
        _reviewers.Sort((x, y) => x.Sequence.CompareTo(y.Sequence));

        return reviewer.Id;
    }

    public void RemoveReviewer(Guid reviewerId)
    {
        EnsurePending();

        var reviewer = FindReviewer(reviewerId);
        _reviewers.Remove(reviewer);
    }

    public void StartReview(DateTime startedAt)
    {
        EnsurePending();

        if (_reviewers.Count == 0)
            throw new ApprovalDomainException(
                "At least one reviewer must be assigned.");

        if (_reviewers.Count(x => x.IsRequired) < RequiredApprovalCount)
            throw new ApprovalDomainException(
                "There are not enough required reviewers to satisfy the approval threshold.");

        ReviewStartedAt = startedAt;
        Status = ApprovalStatus.InReview;

        AddDomainEvent(
            new ApprovalReviewStartedDomainEvent(
                Id,
                string.Join(",", _reviewers.Select(x => x.UserId)),
                startedAt,
                startedAt));
    }

    public void RecordDecision(
        string reviewerUserId,
        ApprovalDecision decision,
        string? comments,
        DateTime decidedAt)
    {
        if (Status != ApprovalStatus.InReview)
            throw new ApprovalDomainException(
                "Approval is not currently under review.");

        if (!Enum.IsDefined(decision))
            throw new ApprovalDomainException("Approval decision is invalid.");

        var normalizedUserId = NormalizeRequired(
            reviewerUserId,
            nameof(reviewerUserId),
            150);

        var reviewer = _reviewers.FirstOrDefault(x =>
            string.Equals(
                x.UserId,
                normalizedUserId,
                StringComparison.Ordinal));

        if (reviewer is null)
            throw new ApprovalDomainException(
                "User is not assigned as a reviewer for this approval.");

        if (reviewer.HasActed)
            throw new ApprovalDomainException(
                "Reviewer has already submitted a decision.");

        if (decision is ApprovalDecision.Reject
            or ApprovalDecision.ReturnForRevision
            && string.IsNullOrWhiteSpace(comments))
        {
            throw new ApprovalDomainException(
                "Comments are required for rejection or return decisions.");
        }

        var record = new ApprovalDecisionRecord(
            Id,
            normalizedUserId,
            decision,
            comments,
            decidedAt);

        reviewer.MarkActed(decidedAt);
        _decisions.Add(record);

        AddDomainEvent(
            new ApprovalDecisionRecordedDomainEvent(
                Id,
                decision,
                normalizedUserId,
                comments,
                decidedAt,
                decidedAt));

        if (decision == ApprovalDecision.Reject)
        {
            Status = ApprovalStatus.Rejected;
            CompletedAt = decidedAt;
            return;
        }

        if (decision == ApprovalDecision.ReturnForRevision)
        {
            Status = ApprovalStatus.Returned;
            CompletedAt = decidedAt;
            return;
        }

        var approvals = _decisions.Count(x =>
            x.Decision == ApprovalDecision.Approve);

        var pendingRequiredReviewers = _reviewers.Any(x =>
            x.IsRequired
            && !x.HasActed);

        if (approvals >= RequiredApprovalCount
            && !pendingRequiredReviewers)
        {
            Status = ApprovalStatus.Approved;
            CompletedAt = decidedAt;
        }
    }

    public void Cancel(
        string cancelledByUserId,
        string? reason,
        DateTime cancelledAt)
    {
        if (Status is ApprovalStatus.Approved
            or ApprovalStatus.Rejected
            or ApprovalStatus.Cancelled)
        {
            throw new ApprovalDomainException(
                "A completed approval cannot be cancelled.");
        }

        var userId = NormalizeRequired(
            cancelledByUserId,
            nameof(cancelledByUserId),
            150);

        if (!string.Equals(
                userId,
                RequestedByUserId,
                StringComparison.Ordinal))
        {
            throw new ApprovalDomainException(
                "Only the requester can cancel this approval.");
        }

        CancellationReason = NormalizeOptional(reason, 2000);
        CancelledAt = cancelledAt;
        Status = ApprovalStatus.Cancelled;

        AddDomainEvent(
            new ApprovalCancelledDomainEvent(
                Id,
                userId,
                CancellationReason,
                cancelledAt,
                cancelledAt));
    }

    private ApprovalReviewer FindReviewer(Guid reviewerId)
    {
        if (reviewerId == Guid.Empty)
            throw new ApprovalDomainException("Reviewer ID is required.");

        return _reviewers.FirstOrDefault(x => x.Id == reviewerId)
            ?? throw new ApprovalDomainException(
                $"Reviewer '{reviewerId}' was not found.");
    }

    private void EnsurePending()
    {
        if (Status != ApprovalStatus.Pending)
            throw new ApprovalDomainException(
                "Reviewers can only be managed while approval is pending.");
    }

    private static string NormalizeRequired(
        string? value,
        string fieldName,
        int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ApprovalDomainException($"{fieldName} is required.");

        if (normalized.Length > maxLength)
            throw new ApprovalDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
            throw new ApprovalDomainException(
                $"Value cannot exceed {maxLength} characters.");

        return normalized;
    }
}
