using LIMS.Domain.Common;
using LIMS.Domain.Approvals.Enums;
using LIMS.Domain.Approvals.Exceptions;

namespace LIMS.Domain.Approvals.Entities;

public sealed class ApprovalDecisionRecord : Entity<Guid>
{
    public Guid ApprovalId { get; private set; }

    public string ReviewerUserId { get; private set; } = string.Empty;

    public ApprovalDecision Decision { get; private set; }

    public string? Comments { get; private set; }

    public DateTime DecidedAt { get; private set; }

    private ApprovalDecisionRecord()
    {
    }

    internal ApprovalDecisionRecord(
        Guid approvalId,
        string reviewerUserId,
        ApprovalDecision decision,
        string? comments,
        DateTime decidedAt)
        : base(Guid.NewGuid())
    {
        if (approvalId == Guid.Empty)
            throw new ApprovalDomainException("Approval ID is required.");

        if (!Enum.IsDefined(decision))
            throw new ApprovalDomainException("Approval decision is invalid.");

        ApprovalId = approvalId;
        ReviewerUserId = NormalizeRequired(
            reviewerUserId,
            nameof(reviewerUserId),
            150);

        Decision = decision;
        Comments = NormalizeOptional(comments, 4000);
        DecidedAt = decidedAt;
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
