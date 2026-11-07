using LIMS.Domain.Common;
using LIMS.Domain.Samples.Enums;
using LIMS.Domain.Samples.Events;
using LIMS.Domain.Samples.Exceptions;

namespace LIMS.Domain.Samples;

public sealed class Sample : AggregateRoot<Guid>
{
    public Guid LaboratoryId { get; private set; }

    public string SampleCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public SampleType Type { get; private set; }

    public SampleStatus Status { get; private set; }

    public SamplePriority Priority { get; private set; }

    public string? Source { get; private set; }

    public string? AssignedToUserId { get; private set; }

    public string? RegisteredByUserId { get; private set; }

    public DateTimeOffset RegisteredAt { get; private set; }

    public DateTimeOffset? ReceivedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    private Sample()
    {
    }

    public static Sample Register(
        Guid laboratoryId,
        string sampleCode,
        string name,
        SampleType type,
        SamplePriority priority,
        string? description,
        string? source,
        string? registeredByUserId,
        DateTimeOffset registeredAt)
    {
        if (laboratoryId == Guid.Empty)
            throw new SampleDomainException("Laboratory identifier is required.");

        if (!Enum.IsDefined(type))
            throw new SampleDomainException("Invalid sample type.");

        if (!Enum.IsDefined(priority))
            throw new SampleDomainException("Invalid sample priority.");

        var sample = new Sample
        {
            Id = Guid.NewGuid(),
            LaboratoryId = laboratoryId,
            SampleCode = NormalizeRequired(sampleCode, nameof(sampleCode), 50),
            Name = NormalizeRequired(name, nameof(name), 200),
            Type = type,
            Priority = priority,
            Description = NormalizeOptional(description, 2000),
            Source = NormalizeOptional(source, 250),
            RegisteredByUserId = NormalizeOptional(registeredByUserId, 150),
            Status = SampleStatus.Registered,
            RegisteredAt = registeredAt
        };

        sample.AddDomainEvent(
            new SampleRegisteredDomainEvent(
                sample.Id,
                sample.LaboratoryId,
                sample.SampleCode,
                registeredAt));

        return sample;
    }

    public void UpdateInformation(
        string name,
        SampleType type,
        SamplePriority priority,
        string? description,
        string? source,
        DateTimeOffset updatedAt)
    {
        EnsureNotArchived();

        if (Status is SampleStatus.UnderAnalysis or SampleStatus.AnalysisCompleted)
        {
            throw new SampleDomainException(
                "Sample information cannot be modified during or after analysis.");
        }

        if (!Enum.IsDefined(type))
            throw new SampleDomainException("Invalid sample type.");

        if (!Enum.IsDefined(priority))
            throw new SampleDomainException("Invalid sample priority.");

        Name = NormalizeRequired(name, nameof(name), 200);
        Type = type;
        Priority = priority;
        Description = NormalizeOptional(description, 2000);
        Source = NormalizeOptional(source, 250);
        UpdatedAt = updatedAt;
    }

    public void Receive(DateTimeOffset receivedAt)
    {
        EnsureNotArchived();

        if (Status != SampleStatus.Registered)
        {
            throw new SampleDomainException(
                "Only registered samples can be received.");
        }

        Status = SampleStatus.Received;
        ReceivedAt = receivedAt;
        UpdatedAt = receivedAt;

        AddStatusChangedEvent(
            SampleStatus.Registered,
            SampleStatus.Received,
            receivedAt);
    }

    public void StartPreparation(DateTimeOffset startedAt)
    {
        ChangeStatus(
            SampleStatus.Received,
            SampleStatus.InPreparation,
            startedAt);
    }

    public void MarkReadyForAnalysis(DateTimeOffset readyAt)
    {
        ChangeStatus(
            SampleStatus.InPreparation,
            SampleStatus.ReadyForAnalysis,
            readyAt);
    }

    public void StartAnalysis(DateTimeOffset startedAt)
    {
        ChangeStatus(
            SampleStatus.ReadyForAnalysis,
            SampleStatus.UnderAnalysis,
            startedAt);
    }

    public void CompleteAnalysis(DateTimeOffset completedAt)
    {
        ChangeStatus(
            SampleStatus.UnderAnalysis,
            SampleStatus.AnalysisCompleted,
            completedAt);
    }

    public void Reject(DateTimeOffset rejectedAt)
    {
        EnsureNotArchived();

        if (Status == SampleStatus.AnalysisCompleted)
        {
            throw new SampleDomainException(
                "A sample with completed analysis cannot be rejected.");
        }

        if (Status == SampleStatus.Rejected)
            return;

        var previousStatus = Status;

        Status = SampleStatus.Rejected;
        UpdatedAt = rejectedAt;

        AddStatusChangedEvent(
            previousStatus,
            SampleStatus.Rejected,
            rejectedAt);
    }

    public void AssignTo(
        string userId,
        DateTimeOffset assignedAt)
    {
        EnsureNotArchived();

        if (Status is SampleStatus.Rejected or SampleStatus.AnalysisCompleted)
        {
            throw new SampleDomainException(
                "This sample cannot be assigned in its current status.");
        }

        if (string.IsNullOrWhiteSpace(userId))
            throw new SampleDomainException("User identifier is required.");

        var normalizedUserId = userId.Trim();

        if (string.Equals(
                AssignedToUserId,
                normalizedUserId,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        AssignedToUserId = normalizedUserId;
        UpdatedAt = assignedAt;

        AddDomainEvent(
            new SampleAssignedDomainEvent(
                Id,
                normalizedUserId,
                assignedAt));
    }

    public void Unassign(DateTimeOffset unassignedAt)
    {
        EnsureNotArchived();

        if (AssignedToUserId is null)
            return;

        AssignedToUserId = null;
        UpdatedAt = unassignedAt;
    }

    public void Archive(DateTimeOffset archivedAt)
    {
        if (Status == SampleStatus.Archived)
            return;

        if (Status is not SampleStatus.AnalysisCompleted
            and not SampleStatus.Rejected)
        {
            throw new SampleDomainException(
                "Only completed or rejected samples can be archived.");
        }

        var previousStatus = Status;

        Status = SampleStatus.Archived;
        ArchivedAt = archivedAt;
        UpdatedAt = archivedAt;

        AddStatusChangedEvent(
            previousStatus,
            SampleStatus.Archived,
            archivedAt);
    }

    private void ChangeStatus(
        SampleStatus expectedStatus,
        SampleStatus newStatus,
        DateTimeOffset changedAt)
    {
        EnsureNotArchived();

        if (Status != expectedStatus)
        {
            throw new SampleDomainException(
                $"Cannot change sample status from '{Status}' to '{newStatus}'.");
        }

        Status = newStatus;
        UpdatedAt = changedAt;

        AddStatusChangedEvent(
            expectedStatus,
            newStatus,
            changedAt);
    }

    private void AddStatusChangedEvent(
        SampleStatus previousStatus,
        SampleStatus newStatus,
        DateTimeOffset occurredOn)
    {
        AddDomainEvent(
            new SampleStatusChangedDomainEvent(
                Id,
                previousStatus,
                newStatus,
                occurredOn));
    }

    private void EnsureNotArchived()
    {
        if (Status == SampleStatus.Archived)
        {
            throw new SampleDomainException(
                "Archived samples cannot be modified.");
        }
    }

    private static string NormalizeRequired(
        string value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new SampleDomainException($"{fieldName} is required.");

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new SampleDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new SampleDomainException(
                $"Value cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}
