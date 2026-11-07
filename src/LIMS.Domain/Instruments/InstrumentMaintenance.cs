using LIMS.Domain.Common;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Domain.Instruments;

public sealed class InstrumentMaintenance : Entity<Guid>
{
    private InstrumentMaintenance()
    {
    }

    internal InstrumentMaintenance(
        Guid instrumentId,
        MaintenanceType type,
        string description,
        DateTimeOffset scheduledAt,
        string scheduledByUserId)
        : base(Guid.NewGuid())
    {
        if (instrumentId == Guid.Empty)
            throw new InstrumentDomainException("InstrumentId is required.");

        if (!Enum.IsDefined(type))
            throw new InstrumentDomainException("Maintenance type is invalid.");

        if (string.IsNullOrWhiteSpace(description))
            throw new InstrumentDomainException("Maintenance description is required.");

        if (description.Trim().Length > 2000)
            throw new InstrumentDomainException("Maintenance description cannot exceed 2000 characters.");

        if (string.IsNullOrWhiteSpace(scheduledByUserId))
            throw new InstrumentDomainException("ScheduledByUserId is required.");

        if (scheduledByUserId.Trim().Length > 150)
            throw new InstrumentDomainException("ScheduledByUserId cannot exceed 150 characters.");

        InstrumentId = instrumentId;
        Type = type;
        Description = description.Trim();
        ScheduledAt = scheduledAt;
        ScheduledByUserId = scheduledByUserId.Trim();
    }

    public Guid InstrumentId { get; private set; }

    public MaintenanceType Type { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public DateTimeOffset ScheduledAt { get; private set; }

    public string ScheduledByUserId { get; private set; } = string.Empty;

    public DateTimeOffset? CompletedAt { get; private set; }

    public string? CompletedByUserId { get; private set; }

    public string? CompletionNotes { get; private set; }

    public bool IsCompleted => CompletedAt.HasValue;

    internal void Complete(
        string completedByUserId,
        string? completionNotes,
        DateTimeOffset completedAt)
    {
        if (IsCompleted)
            throw new InstrumentDomainException("Maintenance has already been completed.");

        if (string.IsNullOrWhiteSpace(completedByUserId))
            throw new InstrumentDomainException("CompletedByUserId is required.");

        if (completedByUserId.Trim().Length > 150)
            throw new InstrumentDomainException("CompletedByUserId cannot exceed 150 characters.");

        if (completionNotes?.Trim().Length > 2000)
            throw new InstrumentDomainException("CompletionNotes cannot exceed 2000 characters.");

        if (completedAt < ScheduledAt)
            throw new InstrumentDomainException("Maintenance cannot be completed before its scheduled time.");

        CompletedAt = completedAt;
        CompletedByUserId = completedByUserId.Trim();
        CompletionNotes = string.IsNullOrWhiteSpace(completionNotes)
            ? null
            : completionNotes.Trim();
    }
}
