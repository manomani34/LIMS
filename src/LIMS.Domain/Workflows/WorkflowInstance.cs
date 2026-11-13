using LIMS.Domain.Common;
using LIMS.Domain.Workflows.Entities;
using LIMS.Domain.Workflows.Enums;
using LIMS.Domain.Workflows.Events;
using LIMS.Domain.Workflows.Exceptions;

namespace LIMS.Domain.Workflows;

public sealed class WorkflowInstance : AggregateRoot<Guid>
{
    private readonly List<WorkflowTask> _tasks = [];

    public Guid WorkflowDefinitionId { get; private set; }

    public int WorkflowDefinitionVersion { get; private set; }

    public string WorkflowName { get; private set; } = string.Empty;

    public string ReferenceType { get; private set; } = string.Empty;

    public Guid ReferenceId { get; private set; }

    public string StartedByUserId { get; private set; } = string.Empty;

    public WorkflowInstanceStatus Status { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<WorkflowTask> Tasks => _tasks.AsReadOnly();

    private WorkflowInstance()
    {
    }

    private WorkflowInstance(
        Guid id,
        Guid workflowDefinitionId,
        int workflowDefinitionVersion,
        string workflowName,
        string referenceType,
        Guid referenceId,
        string startedByUserId,
        DateTime startedAt)
        : base(id)
    {
        if (workflowDefinitionId == Guid.Empty)
            throw new WorkflowDomainException("Workflow definition ID is required.");

        if (workflowDefinitionVersion <= 0)
            throw new WorkflowDomainException("Workflow definition version is invalid.");

        if (referenceId == Guid.Empty)
            throw new WorkflowDomainException("Workflow reference ID is required.");

        WorkflowDefinitionId = workflowDefinitionId;
        WorkflowDefinitionVersion = workflowDefinitionVersion;
        WorkflowName = NormalizeRequired(workflowName, nameof(workflowName), 200);
        ReferenceType = NormalizeRequired(referenceType, nameof(referenceType), 100);
        ReferenceId = referenceId;
        StartedByUserId = NormalizeRequired(
            startedByUserId,
            nameof(startedByUserId),
            150);

        StartedAt = startedAt;
        Status = WorkflowInstanceStatus.InProgress;
    }

    public static WorkflowInstance Start(
        WorkflowDefinition definition,
        string referenceType,
        Guid referenceId,
        string startedByUserId,
        DateTime startedAt)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (definition.Status != WorkflowDefinitionStatus.Active)
            throw new WorkflowDomainException(
                "Only active workflow definitions can be started.");

        if (definition.Steps.Count == 0)
            throw new WorkflowDomainException(
                "Workflow definition has no steps.");

        var instance = new WorkflowInstance(
            Guid.NewGuid(),
            definition.Id,
            definition.Version,
            definition.Name,
            referenceType,
            referenceId,
            startedByUserId,
            startedAt);

        foreach (var step in definition.Steps.OrderBy(x => x.Sequence))
        {
            DateTime? dueAt = step.DueAfterHours.HasValue
    ? startedAt.AddHours(step.DueAfterHours.Value)
    : null;

            instance._tasks.Add(
                new WorkflowTask(
                    instance.Id,
                    step.Id,
                    step.Sequence,
                    step.Code,
                    step.Name,
                    step.Type,
                    step.IsRequired,
                    startedAt,
                    dueAt));
        }

        instance.AddDomainEvent(
            new WorkflowInstanceStartedDomainEvent(
                instance.Id,
                definition.Id,
                instance.StartedByUserId,
                startedAt,
                startedAt));

        return instance;
    }

    public void AssignTask(
        Guid taskId,
        string assignedToUserId,
        DateTime assignedAt)
    {
        EnsureInProgress();

        var task = FindTask(taskId);

        if (task.Status is WorkflowTaskStatus.Completed
            or WorkflowTaskStatus.Rejected
            or WorkflowTaskStatus.Skipped
            or WorkflowTaskStatus.Cancelled)
        {
            throw new WorkflowDomainException(
                "A terminal task cannot be reassigned.");
        }

        task.Assign(assignedToUserId, assignedAt);

        AddDomainEvent(
            new WorkflowTaskAssignedDomainEvent(
                Id,
                task.Id,
                assignedToUserId,
                assignedAt,
                assignedAt));
    }

    public void StartTask(
        Guid taskId,
        string userId,
        DateTime startedAt)
    {
        EnsureInProgress();

        var task = FindTask(taskId);
        EnsurePreviousRequiredTasksCompleted(task);

        task.Start(userId, startedAt);
    }

    public void CompleteTask(
        Guid taskId,
        string userId,
        string? comments,
        DateTime completedAt)
    {
        EnsureInProgress();

        var task = FindTask(taskId);
        EnsurePreviousRequiredTasksCompleted(task);

        task.Complete(userId, comments, completedAt);

        AddDomainEvent(
            new WorkflowTaskCompletedDomainEvent(
                Id,
                task.Id,
                userId,
                completedAt,
                completedAt));

        CompleteWorkflowIfFinished(completedAt);
    }

    public void RejectTask(
        Guid taskId,
        string userId,
        string reason,
        DateTime rejectedAt)
    {
        EnsureInProgress();

        var task = FindTask(taskId);
        EnsurePreviousRequiredTasksCompleted(task);

        task.Reject(userId, reason, rejectedAt);

        AddDomainEvent(
            new WorkflowTaskRejectedDomainEvent(
                Id,
                task.Id,
                userId,
                reason,
                rejectedAt,
                rejectedAt));

        Status = WorkflowInstanceStatus.Rejected;
        CancelRemainingTasks(rejectedAt);
    }

    public void SkipTask(Guid taskId, DateTime skippedAt)
    {
        EnsureInProgress();

        var task = FindTask(taskId);
        EnsurePreviousRequiredTasksCompleted(task);

        task.Skip(skippedAt);
        CompleteWorkflowIfFinished(skippedAt);
    }

    public void Cancel(
        string cancelledByUserId,
        string? reason,
        DateTime cancelledAt)
    {
        if (Status is WorkflowInstanceStatus.Completed
            or WorkflowInstanceStatus.Rejected
            or WorkflowInstanceStatus.Cancelled)
        {
            throw new WorkflowDomainException(
                "A terminal workflow instance cannot be cancelled.");
        }

        var userId = NormalizeRequired(
            cancelledByUserId,
            nameof(cancelledByUserId),
            150);

        CancellationReason = NormalizeOptional(reason, 2000);
        CancelledAt = cancelledAt;
        Status = WorkflowInstanceStatus.Cancelled;

        CancelRemainingTasks(cancelledAt);

        AddDomainEvent(
            new WorkflowInstanceCancelledDomainEvent(
                Id,
                userId,
                CancellationReason,
                cancelledAt,
                cancelledAt));
    }

    public void Suspend()
    {
        EnsureInProgress();
        Status = WorkflowInstanceStatus.Suspended;
    }

    public void Resume()
    {
        if (Status != WorkflowInstanceStatus.Suspended)
            throw new WorkflowDomainException(
                "Only suspended workflows can be resumed.");

        Status = WorkflowInstanceStatus.InProgress;
    }

    private void CompleteWorkflowIfFinished(DateTime completedAt)
    {
        var requiredTasks = _tasks.Where(x => x.IsRequired);

        if (requiredTasks.Any(x =>
                x.Status != WorkflowTaskStatus.Completed
                && x.Status != WorkflowTaskStatus.Skipped))
        {
            return;
        }

        if (_tasks.Any(x =>
                x.Status is WorkflowTaskStatus.Pending
                    or WorkflowTaskStatus.Assigned
                    or WorkflowTaskStatus.InProgress))
        {
            return;
        }

        Status = WorkflowInstanceStatus.Completed;
        CompletedAt = completedAt;

        AddDomainEvent(
            new WorkflowInstanceCompletedDomainEvent(
                Id,
                completedAt,
                completedAt));
    }

    private void EnsurePreviousRequiredTasksCompleted(WorkflowTask task)
    {
        var previousRequiredTasks = _tasks.Where(x =>
            x.IsRequired && x.Sequence < task.Sequence);

        if (previousRequiredTasks.Any(x =>
                x.Status != WorkflowTaskStatus.Completed
                && x.Status != WorkflowTaskStatus.Skipped))
        {
            throw new WorkflowDomainException(
                "All previous required workflow tasks must be completed first.");
        }
    }

    private void CancelRemainingTasks(DateTime cancelledAt)
    {
        foreach (var task in _tasks)
            task.Cancel(cancelledAt);
    }

    private WorkflowTask FindTask(Guid taskId)
    {
        if (taskId == Guid.Empty)
            throw new WorkflowDomainException("Workflow task ID is required.");

        return _tasks.FirstOrDefault(x => x.Id == taskId)
            ?? throw new WorkflowDomainException(
                $"Workflow task '{taskId}' was not found.");
    }

    private void EnsureInProgress()
    {
        if (Status != WorkflowInstanceStatus.InProgress)
            throw new WorkflowDomainException(
                "Workflow instance is not in progress.");
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

