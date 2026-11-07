using LIMS.Domain.Common;
using LIMS.Domain.Workflows.Enums;
using LIMS.Domain.Workflows.Exceptions;

namespace LIMS.Domain.Workflows.Entities;

public sealed class WorkflowTask : Entity<Guid>
{
    public Guid WorkflowInstanceId { get; private set; }

    public Guid WorkflowStepDefinitionId { get; private set; }

    public int Sequence { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public WorkflowStepType Type { get; private set; }

    public bool IsRequired { get; private set; }

    public WorkflowTaskStatus Status { get; private set; }

    public string? AssignedToUserId { get; private set; }

    public string? CompletedByUserId { get; private set; }

    public string? RejectionReason { get; private set; }

    public string? CompletionComments { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? AssignedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? DueAt { get; private set; }

    private WorkflowTask()
    {
    }

    internal WorkflowTask(
        Guid workflowInstanceId,
        Guid workflowStepDefinitionId,
        int sequence,
        string code,
        string name,
        WorkflowStepType type,
        bool isRequired,
        DateTime createdAt,
        DateTime? dueAt)
        : base(Guid.NewGuid())
    {
        if (workflowInstanceId == Guid.Empty)
            throw new WorkflowDomainException("Workflow instance ID is required.");

        if (workflowStepDefinitionId == Guid.Empty)
            throw new WorkflowDomainException("Workflow step definition ID is required.");

        if (sequence <= 0)
            throw new WorkflowDomainException("Task sequence must be greater than zero.");

        if (!Enum.IsDefined(type))
            throw new WorkflowDomainException("Workflow task type is invalid.");

        WorkflowInstanceId = workflowInstanceId;
        WorkflowStepDefinitionId = workflowStepDefinitionId;
        Sequence = sequence;
        Code = NormalizeRequired(code, nameof(code), 50);
        Name = NormalizeRequired(name, nameof(name), 150);
        Type = type;
        IsRequired = isRequired;
        Status = WorkflowTaskStatus.Pending;
        CreatedAt = createdAt;
        DueAt = dueAt;
    }

    internal void Assign(
        string assignedToUserId,
        DateTime assignedAt)
    {
        if (Status is not WorkflowTaskStatus.Pending
            and not WorkflowTaskStatus.Assigned)
        {
            throw new WorkflowDomainException(
                "Only pending or assigned tasks can be assigned.");
        }

        AssignedToUserId = NormalizeRequired(
            assignedToUserId,
            nameof(assignedToUserId),
            150);

        AssignedAt = assignedAt;
        Status = WorkflowTaskStatus.Assigned;
    }

    internal void Start(
        string userId,
        DateTime startedAt)
    {
        EnsureAssignedUser(userId);

        if (Status != WorkflowTaskStatus.Assigned)
            throw new WorkflowDomainException(
                "Only assigned tasks can be started.");

        StartedAt = startedAt;
        Status = WorkflowTaskStatus.InProgress;
    }

    internal void Complete(
        string userId,
        string? comments,
        DateTime completedAt)
    {
        EnsureAssignedUser(userId);

        if (Status is not WorkflowTaskStatus.Assigned
            and not WorkflowTaskStatus.InProgress)
        {
            throw new WorkflowDomainException(
                "Only assigned or in-progress tasks can be completed.");
        }

        CompletedByUserId = NormalizeRequired(
            userId,
            nameof(userId),
            150);

        CompletionComments = NormalizeOptional(comments, 2000);
        CompletedAt = completedAt;
        Status = WorkflowTaskStatus.Completed;
    }

    internal void Reject(
        string userId,
        string reason,
        DateTime rejectedAt)
    {
        EnsureAssignedUser(userId);

        if (Status is not WorkflowTaskStatus.Assigned
            and not WorkflowTaskStatus.InProgress)
        {
            throw new WorkflowDomainException(
                "Only assigned or in-progress tasks can be rejected.");
        }

        RejectionReason = NormalizeRequired(reason, nameof(reason), 2000);
        CompletedByUserId = NormalizeRequired(
            userId,
            nameof(userId),
            150);

        CompletedAt = rejectedAt;
        Status = WorkflowTaskStatus.Rejected;
    }

    internal void Skip(DateTime skippedAt)
    {
        if (IsRequired)
            throw new WorkflowDomainException(
                "Required workflow tasks cannot be skipped.");

        if (Status is WorkflowTaskStatus.Completed
            or WorkflowTaskStatus.Rejected
            or WorkflowTaskStatus.Cancelled)
        {
            throw new WorkflowDomainException(
                "A terminal workflow task cannot be skipped.");
        }

        CompletedAt = skippedAt;
        Status = WorkflowTaskStatus.Skipped;
    }

    internal void Cancel(DateTime cancelledAt)
    {
        if (Status is WorkflowTaskStatus.Completed
            or WorkflowTaskStatus.Rejected
            or WorkflowTaskStatus.Skipped)
        {
            return;
        }

        CompletedAt = cancelledAt;
        Status = WorkflowTaskStatus.Cancelled;
    }

    private void EnsureAssignedUser(string userId)
    {
        var normalized = NormalizeRequired(userId, nameof(userId), 150);

        if (!string.Equals(
                AssignedToUserId,
                normalized,
                StringComparison.Ordinal))
        {
            throw new WorkflowDomainException(
                "Only the assigned user can perform this task operation.");
        }
    }

    private static string NormalizeRequired(
        string? value,
        string fieldName,
        int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new WorkflowDomainException($"{fieldName} is required.");

        if (normalized.Length > maxLength)
            throw new WorkflowDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
            throw new WorkflowDomainException(
                $"Value cannot exceed {maxLength} characters.");

        return normalized;
    }
}
