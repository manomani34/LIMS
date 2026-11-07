using LIMS.Domain.Common;
using LIMS.Domain.Workflows.Entities;
using LIMS.Domain.Workflows.Enums;
using LIMS.Domain.Workflows.Events;
using LIMS.Domain.Workflows.Exceptions;

namespace LIMS.Domain.Workflows;

public sealed class WorkflowDefinition : AggregateRoot<Guid>
{
    private readonly List<WorkflowStepDefinition> _steps = [];

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string CreatedByUserId { get; private set; } = string.Empty;

    public WorkflowDefinitionStatus Status { get; private set; }

    public int Version { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? ActivatedAt { get; private set; }

    public DateTime? DeprecatedAt { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public IReadOnlyCollection<WorkflowStepDefinition> Steps => _steps.AsReadOnly();

    private WorkflowDefinition()
    {
    }

    private WorkflowDefinition(
        Guid id,
        string code,
        string name,
        string? description,
        string createdByUserId,
        int version,
        DateTime createdAt)
        : base(id)
    {
        Code = NormalizeRequired(code, nameof(code), 50).ToUpperInvariant();
        ValidateCode(Code);
        Name = NormalizeRequired(name, nameof(name), 200);
        Description = NormalizeOptional(description, 2000);
        CreatedByUserId = NormalizeRequired(
            createdByUserId,
            nameof(createdByUserId),
            150);

        if (version <= 0)
            throw new WorkflowDomainException("Workflow version must be greater than zero.");

        Version = version;
        Status = WorkflowDefinitionStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public static WorkflowDefinition Create(
        string code,
        string name,
        string? description,
        string createdByUserId,
        int version,
        DateTime createdAt)
    {
        var definition = new WorkflowDefinition(
            Guid.NewGuid(),
            code,
            name,
            description,
            createdByUserId,
            version,
            createdAt);

        definition.AddDomainEvent(
            new WorkflowDefinitionCreatedDomainEvent(
                definition.Id,
                definition.Code,
                definition.Name,
                createdAt));

        return definition;
    }

    public void UpdateInformation(
        string name,
        string? description,
        DateTime updatedAt)
    {
        EnsureDraft();

        Name = NormalizeRequired(name, nameof(name), 200);
        Description = NormalizeOptional(description, 2000);
        UpdatedAt = updatedAt;
    }

    public Guid AddStep(
        int sequence,
        string code,
        string name,
        string? description,
        WorkflowStepType type,
        bool isRequired,
        bool allowReassignment,
        int? dueAfterHours,
        string? requiredRole,
        DateTime updatedAt)
    {
        EnsureDraft();

        if (_steps.Any(x => x.Sequence == sequence))
            throw new WorkflowDomainException(
                $"A workflow step with sequence '{sequence}' already exists.");

        if (_steps.Any(x => string.Equals(
                x.Code,
                code?.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new WorkflowDomainException(
                $"A workflow step with code '{code}' already exists.");
        }

        var step = new WorkflowStepDefinition(
            Id,
            sequence,
            code,
            name,
            description,
            type,
            isRequired,
            allowReassignment,
            dueAfterHours,
            requiredRole);

        _steps.Add(step);
        _steps.Sort((left, right) => left.Sequence.CompareTo(right.Sequence));
        UpdatedAt = updatedAt;

        return step.Id;
    }

    public void UpdateStep(
        Guid stepId,
        int sequence,
        string code,
        string name,
        string? description,
        WorkflowStepType type,
        bool isRequired,
        bool allowReassignment,
        int? dueAfterHours,
        string? requiredRole,
        DateTime updatedAt)
    {
        EnsureDraft();

        var step = FindStep(stepId);

        if (_steps.Any(x =>
                x.Id != stepId && x.Sequence == sequence))
        {
            throw new WorkflowDomainException(
                $"A workflow step with sequence '{sequence}' already exists.");
        }

        if (_steps.Any(x =>
                x.Id != stepId
                && string.Equals(
                    x.Code,
                    code?.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new WorkflowDomainException(
                $"A workflow step with code '{code}' already exists.");
        }

        step.Update(
            sequence,
            code,
            name,
            description,
            type,
            isRequired,
            allowReassignment,
            dueAfterHours,
            requiredRole);

        _steps.Sort((left, right) => left.Sequence.CompareTo(right.Sequence));
        UpdatedAt = updatedAt;
    }

    public void RemoveStep(Guid stepId, DateTime updatedAt)
    {
        EnsureDraft();

        var step = FindStep(stepId);
        _steps.Remove(step);
        UpdatedAt = updatedAt;
    }

    public void Activate(DateTime activatedAt)
    {
        EnsureDraft();

        if (_steps.Count == 0)
            throw new WorkflowDomainException(
                "A workflow definition must contain at least one step before activation.");

        if (!_steps.Any(x => x.IsRequired))
            throw new WorkflowDomainException(
                "A workflow definition must contain at least one required step.");

        ActivatedAt = activatedAt;
        Status = WorkflowDefinitionStatus.Active;
        UpdatedAt = activatedAt;

        AddDomainEvent(
            new WorkflowDefinitionActivatedDomainEvent(
                Id,
                activatedAt,
                activatedAt));
    }

    public void Deprecate(DateTime deprecatedAt)
    {
        if (Status != WorkflowDefinitionStatus.Active)
            throw new WorkflowDomainException(
                "Only active workflow definitions can be deprecated.");

        Status = WorkflowDefinitionStatus.Deprecated;
        DeprecatedAt = deprecatedAt;
        UpdatedAt = deprecatedAt;
    }

    public void Archive(DateTime archivedAt)
    {
        if (Status != WorkflowDefinitionStatus.Deprecated)
            throw new WorkflowDomainException(
                "Only deprecated workflow definitions can be archived.");

        Status = WorkflowDefinitionStatus.Archived;
        ArchivedAt = archivedAt;
        UpdatedAt = archivedAt;
    }

    private WorkflowStepDefinition FindStep(Guid stepId)
    {
        if (stepId == Guid.Empty)
            throw new WorkflowDomainException("Workflow step ID is required.");

        return _steps.FirstOrDefault(x => x.Id == stepId)
            ?? throw new WorkflowDomainException(
                $"Workflow step '{stepId}' was not found.");
    }

    private void EnsureDraft()
    {
        if (Status != WorkflowDefinitionStatus.Draft)
            throw new WorkflowDomainException(
                "Only draft workflow definitions can be modified.");
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

    private static void ValidateCode(string code)
    {
        if (code.Any(character =>
                !char.IsAsciiLetterOrDigit(character)
                && character != '-'
                && character != '_'))
        {
            throw new WorkflowDomainException(
                "Workflow code can contain only letters, numbers, hyphens and underscores.");
        }
    }
}
