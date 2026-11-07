using LIMS.Domain.Common;
using LIMS.Domain.Approvals.Exceptions;

namespace LIMS.Domain.Approvals.Entities;

public sealed class ApprovalReviewer : Entity<Guid>
{
    public Guid ApprovalId { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public int Sequence { get; private set; }

    public bool IsRequired { get; private set; }

    public bool HasActed { get; private set; }

    public DateTime AssignedAt { get; private set; }

    public DateTime? ActedAt { get; private set; }

    private ApprovalReviewer()
    {
    }

    internal ApprovalReviewer(
        Guid approvalId,
        string userId,
        int sequence,
        bool isRequired,
        DateTime assignedAt)
        : base(Guid.NewGuid())
    {
        if (approvalId == Guid.Empty)
            throw new ApprovalDomainException("Approval ID is required.");

        if (sequence <= 0)
            throw new ApprovalDomainException("Reviewer sequence must be greater than zero.");

        ApprovalId = approvalId;
        UserId = NormalizeRequired(userId, nameof(userId), 150);
        Sequence = sequence;
        IsRequired = isRequired;
        AssignedAt = assignedAt;
    }

    internal void MarkActed(DateTime actedAt)
    {
        if (HasActed)
            throw new ApprovalDomainException(
                "Reviewer has already submitted a decision.");

        HasActed = true;
        ActedAt = actedAt;
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
}
