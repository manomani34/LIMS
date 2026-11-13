using LIMS.Domain.Common;
using LIMS.Domain.Instruments.Enums;
using LIMS.Domain.Instruments.Events;

namespace LIMS.Domain.Instruments;

public sealed class Instrument : AggregateRoot<Guid>
{
    private readonly List<InstrumentMaintenance> _maintenanceRecords = [];

    private Instrument()
    {
    }

    private Instrument(Guid id)
        : base(id)
    {
    }

    public Guid LaboratoryId { get; private set; }

    public string InstrumentCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Manufacturer { get; private set; }

    public string? Model { get; private set; }

    public string? SerialNumber { get; private set; }

    public InstrumentType Type { get; private set; }

    public InstrumentStatus Status { get; private set; }

    public string? Location { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset RegisteredAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? RetiredAt { get; private set; }

    public IReadOnlyCollection<InstrumentMaintenance> MaintenanceRecords =>
        _maintenanceRecords.AsReadOnly();

    public static Instrument Register(
        Guid laboratoryId,
        string instrumentCode,
        string name,
        InstrumentType type,
        string? manufacturer,
        string? model,
        string? serialNumber,
        string? location,
        string? description,
        DateTimeOffset registeredAt)
    {
        if (laboratoryId == Guid.Empty)
            throw new InstrumentDomainException("LaboratoryId is required.");

        var normalizedCode = NormalizeCode(instrumentCode);
        var normalizedName = NormalizeRequiredText(name, 200, "Name");

        if (!Enum.IsDefined(type))
            throw new InstrumentDomainException("Instrument type is invalid.");

        var instrument = new Instrument(Guid.NewGuid())
        {
            LaboratoryId = laboratoryId,
            InstrumentCode = normalizedCode,
            Name = normalizedName,
            Type = type,
            Manufacturer = NormalizeOptionalText(manufacturer, 150, "Manufacturer"),
            Model = NormalizeOptionalText(model, 150, "Model"),
            SerialNumber = NormalizeOptionalText(serialNumber, 150, "SerialNumber"),
            Location = NormalizeOptionalText(location, 250, "Location"),
            Description = NormalizeOptionalText(description, 2000, "Description"),
            Status = InstrumentStatus.Available,
            RegisteredAt = registeredAt,
            UpdatedAt = registeredAt
        };

        instrument.AddDomainEvent(
            new InstrumentRegisteredDomainEvent(
                instrument.Id,
                laboratoryId,
                normalizedCode,
                registeredAt));

        return instrument;
    }

    public void UpdateInformation(
        string name,
        InstrumentType type,
        string? manufacturer,
        string? model,
        string? serialNumber,
        string? location,
        string? description,
        DateTimeOffset updatedAt)
    {
        EnsureNotRetired();

        if (!Enum.IsDefined(type))
            throw new InstrumentDomainException("Instrument type is invalid.");

        Name = NormalizeRequiredText(name, 200, "Name");
        Type = type;
        Manufacturer = NormalizeOptionalText(manufacturer, 150, "Manufacturer");
        Model = NormalizeOptionalText(model, 150, "Model");
        SerialNumber = NormalizeOptionalText(serialNumber, 150, "SerialNumber");
        Location = NormalizeOptionalText(location, 250, "Location");
        Description = NormalizeOptionalText(description, 2000, "Description");
        UpdatedAt = updatedAt;
    }

    public void MarkInUse(DateTimeOffset occurredOn)
    {
        EnsureNotRetired();

        if (Status != InstrumentStatus.Available)
            throw new InstrumentDomainException(
                "Only available instruments can be marked as in use.");

        ChangeStatus(InstrumentStatus.InUse, occurredOn);
        UpdatedAt = occurredOn;
    }

    public void MarkAvailable(DateTimeOffset occurredOn)
    {
        EnsureNotRetired();

        if (Status != InstrumentStatus.InUse)
            throw new InstrumentDomainException(
                "Only instruments in use can be marked as available.");

        ChangeStatus(InstrumentStatus.Available, occurredOn);
        UpdatedAt = occurredOn;
    }

    public void ScheduleMaintenance(
        MaintenanceType type,
        string description,
        DateTimeOffset scheduledAt,
        string scheduledByUserId,
        DateTimeOffset occurredOn)
    {
        EnsureNotRetired();

        if (Status == InstrumentStatus.InUse)
            throw new InstrumentDomainException("Maintenance cannot be scheduled while the instrument is in use.");

        if (_maintenanceRecords.Any(x => !x.IsCompleted))
            throw new InstrumentDomainException("An active maintenance record already exists.");

        var maintenance = new InstrumentMaintenance(
            Id,
            type,
            description,
            scheduledAt,
            scheduledByUserId);

        _maintenanceRecords.Add(maintenance);

        ChangeStatus(InstrumentStatus.UnderMaintenance, occurredOn);

        AddDomainEvent(
            new InstrumentMaintenanceScheduledDomainEvent(
                Id,
                maintenance.Id,
                type,
                scheduledAt,
                occurredOn));

        UpdatedAt = occurredOn;
    }

    public void CompleteMaintenance(
        Guid maintenanceId,
        string completedByUserId,
        string? completionNotes,
        DateTimeOffset completedAt)
    {
        var maintenance = _maintenanceRecords.FirstOrDefault(x => x.Id == maintenanceId)
            ?? throw new InstrumentDomainException("Maintenance record was not found.");

        maintenance.Complete(
            completedByUserId,
            completionNotes,
            completedAt);

        AddDomainEvent(
            new InstrumentMaintenanceCompletedDomainEvent(
                Id,
                maintenanceId,
                completedAt,
                completedAt));

        if (!_maintenanceRecords.Any(x => !x.IsCompleted))
            ChangeStatus(InstrumentStatus.Available, completedAt);

        UpdatedAt = completedAt;
    }

    public void MarkOutOfService(string reason, DateTimeOffset occurredOn)
    {
        EnsureNotRetired();

        if (string.IsNullOrWhiteSpace(reason))
            throw new InstrumentDomainException("A reason is required.");

        if (reason.Trim().Length > 1000)
            throw new InstrumentDomainException("Reason cannot exceed 1000 characters.");

        if (Status == InstrumentStatus.OutOfService)
            throw new InstrumentDomainException("Instrument is already out of service.");

        ChangeStatus(InstrumentStatus.OutOfService, occurredOn);
        UpdatedAt = occurredOn;
    }

    public void RestoreService(DateTimeOffset occurredOn)
    {
        EnsureNotRetired();

        if (Status != InstrumentStatus.OutOfService)
            throw new InstrumentDomainException("Only out-of-service instruments can be restored.");

        if (_maintenanceRecords.Any(x => !x.IsCompleted))
            throw new InstrumentDomainException("Active maintenance must be completed before restoring the instrument.");

        ChangeStatus(InstrumentStatus.Available, occurredOn);
        UpdatedAt = occurredOn;
    }

    public void Retire(DateTimeOffset retiredAt)
    {
        if (Status == InstrumentStatus.Retired)
            throw new InstrumentDomainException("Instrument is already retired.");

        if (Status == InstrumentStatus.InUse)
            throw new InstrumentDomainException("An instrument in use cannot be retired.");

        if (_maintenanceRecords.Any(x => !x.IsCompleted))
            throw new InstrumentDomainException("Active maintenance must be completed before retiring the instrument.");

        RetiredAt = retiredAt;
        ChangeStatus(InstrumentStatus.Retired, retiredAt);
        UpdatedAt = retiredAt;
    }

    private void ChangeStatus(
        InstrumentStatus newStatus,
        DateTimeOffset occurredOn)
    {
        if (Status == newStatus)
            return;

        var previousStatus = Status;
        Status = newStatus;

        AddDomainEvent(
            new InstrumentStatusChangedDomainEvent(
                Id,
                previousStatus,
                newStatus,
                occurredOn));
    }

    private void EnsureNotRetired()
    {
        if (Status == InstrumentStatus.Retired)
            throw new InstrumentDomainException("Retired instruments cannot be modified.");
    }

    private static string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InstrumentDomainException("InstrumentCode is required.");

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > 50)
            throw new InstrumentDomainException("InstrumentCode cannot exceed 50 characters.");

        if (normalized.Any(c =>
                !char.IsAsciiLetterOrDigit(c) &&
                c != '-' &&
                c != '_'))
        {
            throw new InstrumentDomainException(
                "InstrumentCode can contain only letters, numbers, hyphens and underscores.");
        }

        return normalized;
    }

    private static string NormalizeRequiredText(
        string value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InstrumentDomainException($"{fieldName} is required.");

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
            throw new InstrumentDomainException($"{fieldName} cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static string? NormalizeOptionalText(
        string? value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
            throw new InstrumentDomainException($"{fieldName} cannot exceed {maxLength} characters.");

        return normalized;
    }
}
