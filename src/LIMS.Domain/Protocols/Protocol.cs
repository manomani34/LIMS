using LIMS.Domain.Common;
using LIMS.Domain.Protocols.Enums;
using LIMS.Domain.Protocols.Events;

namespace LIMS.Domain.Protocols;

public sealed class Protocol : AggregateRoot<Guid>
{
    private readonly List<ProtocolStep> _steps = [];

    public Guid LaboratoryId { get; private set; }

    public string ProtocolCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? Version { get; private set; }

    public ProtocolStatus Status { get; private set; }

    public string CreatedByUserId { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? ActivatedAt { get; private set; }

    public DateTimeOffset? DeprecatedAt { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    public IReadOnlyCollection<ProtocolStep> Steps => _steps.AsReadOnly();

    private Protocol()
    {
    }

    private Protocol(
        Guid id,
        Guid laboratoryId,
        string protocolCode,
        string name,
        string? description,
        string? version,
        string createdByUserId,
        DateTimeOffset createdAt)
        : base(id)
    {
        if (laboratoryId == Guid.Empty)
            throw new ProtocolDomainException("LaboratoryId is required.");

        LaboratoryId = laboratoryId;
        ProtocolCode = NormalizeCode(protocolCode);
        Name = NormalizeRequiredText(name, 200, "Protocol name");
        Description = NormalizeOptionalText(description, 4000, "Description");
        Version = NormalizeOptionalText(version, 50, "Version");
        CreatedByUserId = NormalizeRequiredText(
            createdByUserId,
            150,
            "CreatedByUserId");

        Status = ProtocolStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;

        AddDomainEvent(new ProtocolCreatedDomainEvent(
            Id,
            LaboratoryId,
            ProtocolCode,
            createdAt));
    }

    public static Protocol Create(
        Guid laboratoryId,
        string protocolCode,
        string name,
        string? description,
        string? version,
        string createdByUserId,
        DateTimeOffset createdAt)
    {
        return new Protocol(
            Guid.NewGuid(),
            laboratoryId,
            protocolCode,
            name,
            description,
            version,
            createdByUserId,
            createdAt);
    }

    public void UpdateInformation(
        string name,
        string? description,
        string? version,
        DateTimeOffset updatedAt)
    {
        EnsureDraft();

        Name = NormalizeRequiredText(name, 200, "Protocol name");
        Description = NormalizeOptionalText(description, 4000, "Description");
        Version = NormalizeOptionalText(version, 50, "Version");
        UpdatedAt = updatedAt;

        AddDomainEvent(new ProtocolUpdatedDomainEvent(Id, updatedAt));
    }

    public Guid AddStep(
        int sequence,
        string title,
        string instruction,
        ProtocolStepType type,
        bool isRequired,
        decimal? targetValue,
        string? targetUnit,
        decimal? minimumValue,
        decimal? maximumValue,
        string? notes,
        DateTimeOffset occurredAt)
    {
        EnsureDraft();

        if (_steps.Any(x => x.Sequence == sequence))
            throw new ProtocolDomainException(
                $"A step with sequence '{sequence}' already exists.");

        var step = new ProtocolStep(
            Id,
            sequence,
            title,
            instruction,
            type,
            isRequired,
            targetValue,
            targetUnit,
            minimumValue,
            maximumValue,
            notes);

        _steps.Add(step);
        _steps.Sort((left, right) => left.Sequence.CompareTo(right.Sequence));
        UpdatedAt = occurredAt;

        AddDomainEvent(new ProtocolStepAddedDomainEvent(
            Id,
            step.Id,
            step.Sequence,
            occurredAt));

        return step.Id;
    }

    public void UpdateStep(
        Guid stepId,
        int sequence,
        string title,
        string instruction,
        ProtocolStepType type,
        bool isRequired,
        decimal? targetValue,
        string? targetUnit,
        decimal? minimumValue,
        decimal? maximumValue,
        string? notes,
        DateTimeOffset occurredAt)
    {
        EnsureDraft();

        var step = GetStep(stepId);

        if (_steps.Any(x => x.Id != stepId && x.Sequence == sequence))
            throw new ProtocolDomainException(
                $"A step with sequence '{sequence}' already exists.");

        step.Update(
            sequence,
            title,
            instruction,
            type,
            isRequired,
            targetValue,
            targetUnit,
            minimumValue,
            maximumValue,
            notes);

        _steps.Sort((left, right) => left.Sequence.CompareTo(right.Sequence));
        UpdatedAt = occurredAt;

        AddDomainEvent(new ProtocolStepUpdatedDomainEvent(
            Id,
            step.Id,
            occurredAt));
    }

    public void RemoveStep(Guid stepId, DateTimeOffset occurredAt)
    {
        EnsureDraft();

        var step = GetStep(stepId);

        _steps.Remove(step);
        UpdatedAt = occurredAt;

        AddDomainEvent(new ProtocolStepRemovedDomainEvent(
            Id,
            stepId,
            occurredAt));
    }

    public void Activate(DateTimeOffset activatedAt)
    {
        if (Status != ProtocolStatus.Draft)
            throw new ProtocolDomainException(
                "Only draft protocols can be activated.");

        if (_steps.Count == 0)
            throw new ProtocolDomainException(
                "A protocol must contain at least one step before activation.");

        Status = ProtocolStatus.Active;
        ActivatedAt = activatedAt;
        UpdatedAt = activatedAt;

        AddDomainEvent(new ProtocolStatusChangedDomainEvent(
            Id,
            ProtocolStatus.Draft,
            ProtocolStatus.Active,
            activatedAt));
    }

    public void Deprecate(DateTimeOffset deprecatedAt)
    {
        if (Status != ProtocolStatus.Active)
            throw new ProtocolDomainException(
                "Only active protocols can be deprecated.");

        Status = ProtocolStatus.Deprecated;
        DeprecatedAt = deprecatedAt;
        UpdatedAt = deprecatedAt;

        AddDomainEvent(new ProtocolStatusChangedDomainEvent(
            Id,
            ProtocolStatus.Active,
            ProtocolStatus.Deprecated,
            deprecatedAt));
    }

    public void Archive(DateTimeOffset archivedAt)
    {
        if (Status != ProtocolStatus.Deprecated)
            throw new ProtocolDomainException(
                "Only deprecated protocols can be archived.");

        Status = ProtocolStatus.Archived;
        ArchivedAt = archivedAt;
        UpdatedAt = archivedAt;

        AddDomainEvent(new ProtocolStatusChangedDomainEvent(
            Id,
            ProtocolStatus.Deprecated,
            ProtocolStatus.Archived,
            archivedAt));
    }

    private ProtocolStep GetStep(Guid stepId)
    {
        if (stepId == Guid.Empty)
            throw new ProtocolDomainException("StepId is required.");

        return _steps.FirstOrDefault(x => x.Id == stepId)
            ?? throw new ProtocolDomainException(
                $"Protocol step '{stepId}' was not found.");
    }

    private void EnsureDraft()
    {
        if (Status != ProtocolStatus.Draft)
            throw new ProtocolDomainException(
                "Only draft protocols can be modified.");
    }

    private static string NormalizeCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ProtocolDomainException("Protocol code is required.");

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > 50)
            throw new ProtocolDomainException(
                "Protocol code cannot exceed 50 characters.");

        if (normalized.Any(c =>
                !char.IsAsciiLetterOrDigit(c) &&
                c != '-' &&
                c != '_'))
        {
            throw new ProtocolDomainException(
                "Protocol code can contain only letters, numbers, hyphens and underscores.");
        }

        return normalized;
    }

    private static string NormalizeRequiredText(
        string? value,
        int maxLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ProtocolDomainException($"{fieldName} is required.");

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
            throw new ProtocolDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");

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
            throw new ProtocolDomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");

        return normalized;
    }
}
