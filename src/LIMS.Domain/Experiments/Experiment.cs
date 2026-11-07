using LIMS.Domain.Common;
using LIMS.Domain.Experiments.Enums;
using LIMS.Domain.Experiments.Events;

namespace LIMS.Domain.Experiments;

public sealed class Experiment : AggregateRoot<Guid>
{
    public Guid LaboratoryId { get; private set; }

    public Guid SampleId { get; private set; }

    public string ExperimentCode { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public ExperimentStatus Status { get; private set; }

    public string CreatedByUserId { get; private set; } = string.Empty;

    public string? AssignedToUserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    private Experiment()
    {
    }

    private Experiment(
        Guid id,
        Guid laboratoryId,
        Guid sampleId,
        string experimentCode,
        string title,
        string? description,
        string createdByUserId,
        DateTimeOffset createdAt) : base(id)
    {
        LaboratoryId = laboratoryId;
        SampleId = sampleId;
        ExperimentCode = NormalizeCode(experimentCode);
        Title = NormalizeRequiredText(title, nameof(title), 200);
        Description = NormalizeOptionalText(description, 2000);
        CreatedByUserId = NormalizeRequiredText(
            createdByUserId,
            nameof(createdByUserId),
            150);

        Status = ExperimentStatus.Planned;
        CreatedAt = createdAt;
    }

    public static Experiment Create(
        Guid laboratoryId,
        Guid sampleId,
        string experimentCode,
        string title,
        string? description,
        string createdByUserId,
        DateTimeOffset createdAt)
    {
        if (laboratoryId == Guid.Empty)
            throw new ExperimentDomainException("Laboratory identifier is required.");

        if (sampleId == Guid.Empty)
            throw new ExperimentDomainException("Sample identifier is required.");

        var experiment = new Experiment(
            Guid.NewGuid(),
            laboratoryId,
            sampleId,
            experimentCode,
            title,
            description,
            createdByUserId,
            createdAt);

        experiment.AddDomainEvent(new ExperimentCreatedDomainEvent(
            experiment.Id,
            experiment.LaboratoryId,
            experiment.SampleId,
            experiment.ExperimentCode,
            createdAt));

        return experiment;
    }

    public void UpdateInformation(
        string title,
        string? description,
        DateTimeOffset updatedAt)
    {
        EnsureEditable();

        Title = NormalizeRequiredText(title, nameof(title), 200);
        Description = NormalizeOptionalText(description, 2000);
        UpdatedAt = updatedAt;
    }

    public void AssignTo(string userId, DateTimeOffset assignedAt)
    {
        EnsureNotTerminal();

        AssignedToUserId = NormalizeRequiredText(
            userId,
            nameof(userId),
            150);

        UpdatedAt = assignedAt;
    }

    public void Unassign(DateTimeOffset updatedAt)
    {
        EnsureNotTerminal();

        AssignedToUserId = null;
        UpdatedAt = updatedAt;
    }

    public void Start(DateTimeOffset startedAt)
    {
        if (Status != ExperimentStatus.Planned)
        {
            throw new ExperimentDomainException(
                "Only planned experiments can be started.");
        }

        Status = ExperimentStatus.InProgress;
        StartedAt = startedAt;
        UpdatedAt = startedAt;

        AddDomainEvent(new ExperimentStartedDomainEvent(
            Id,
            SampleId,
            startedAt,
            startedAt));
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (Status != ExperimentStatus.InProgress)
        {
            throw new ExperimentDomainException(
                "Only in-progress experiments can be completed.");
        }

        if (StartedAt.HasValue && completedAt < StartedAt.Value)
        {
            throw new ExperimentDomainException(
                "Completion time cannot be earlier than start time.");
        }

        Status = ExperimentStatus.Completed;
        CompletedAt = completedAt;
        UpdatedAt = completedAt;

        AddDomainEvent(new ExperimentCompletedDomainEvent(
            Id,
            SampleId,
            completedAt,
            completedAt));
    }

    public void Cancel(string reason, DateTimeOffset cancelledAt)
    {
        if (Status is ExperimentStatus.Completed
            or ExperimentStatus.Cancelled
            or ExperimentStatus.Archived)
        {
            throw new ExperimentDomainException(
                "This experiment cannot be cancelled in its current status.");
        }

        var normalizedReason = NormalizeRequiredText(
            reason,
            nameof(reason),
            1000);

        Status = ExperimentStatus.Cancelled;
        UpdatedAt = cancelledAt;

        AddDomainEvent(new ExperimentCancelledDomainEvent(
            Id,
            SampleId,
            normalizedReason,
            cancelledAt));
    }

    public void Archive(DateTimeOffset archivedAt)
    {
        if (Status is not (ExperimentStatus.Completed or ExperimentStatus.Cancelled))
        {
            throw new ExperimentDomainException(
                "Only completed or cancelled experiments can be archived.");
        }

        Status = ExperimentStatus.Archived;
        ArchivedAt = archivedAt;
        UpdatedAt = archivedAt;

        AddDomainEvent(new ExperimentArchivedDomainEvent(
            Id,
            archivedAt));
    }

    private void EnsureEditable()
    {
        if (Status != ExperimentStatus.Planned)
        {
            throw new ExperimentDomainException(
                "Only planned experiments can be modified.");
        }
    }

    private void EnsureNotTerminal()
    {
        if (Status is ExperimentStatus.Completed
            or ExperimentStatus.Cancelled
            or ExperimentStatus.Archived)
        {
            throw new ExperimentDomainException(
                "This experiment cannot be modified in its current status.");
        }
    }

    private static string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ExperimentDomainException("Experiment code is required.");

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > 50)
            throw new ExperimentDomainException(
                "Experiment code cannot exceed 50 characters.");

        if (normalized.Any(character =>
                !char.IsLetterOrDigit(character)
                && character is not '-' and not '_'))
        {
            throw new ExperimentDomainException(
                "Experiment code can contain only letters, numbers, hyphens and underscores.");
        }

        return normalized;
    }

    private static string NormalizeRequiredText(
        string value,
        string parameterName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ExperimentDomainException(
                $"{parameterName} is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ExperimentDomainException(
                $"{parameterName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ExperimentDomainException(
                $"Description cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}
