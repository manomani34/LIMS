using LIMS.Domain.Common;
using LIMS.Domain.Workflows.Enums;
using LIMS.Domain.Workflows.Exceptions;

namespace LIMS.Domain.Workflows.Entities;

public sealed class WorkflowStepDefinition : Entity<Guid>
{
    public Guid WorkflowDefinitionId { get; private set; }

    public int Sequence { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public WorkflowStepType Type { get; private set; }

    public bool IsRequired { get; private set; }

    public bool AllowReassignment { get; private set; }

    public int? DueAfterHours { get; private set; }

    public string? RequiredRole { get; private set; }

    private WorkflowStepDefinition()
    {
    }

    internal WorkflowStepDefinition(
        Guid workflowDefinitionId,
        int sequence,
        string code,
        string name,
        string? description,
        WorkflowStepType type,
        bool isRequired,
        bool allowReassignment,
        int? dueAfterHours,
        string? requiredRole)
        : base(Guid.NewGuid())
    {
        if (workflowDefinitionId == Guid.Empty)
            throw new WorkflowDomainException("Workflow definition ID is required.");

        if (sequence <= 0)
            throw new WorkflowDomainException("Step sequence must be greater than zero.");

        WorkflowDefinitionId = workflowDefinitionId;
        Sequence = sequence;
        Code = NormalizeRequired(code, nameof(code), 50).ToUpperInvariant();
        ValidateCode(Code);
        Name = NormalizeRequired(name, nameof(name), 150);
        Description = NormalizeOptional(description, 1000);

        if (!Enum.IsDefined(type))
            throw new WorkflowDomainException("Workflow step type is invalid.");

        if (dueAfterHours is <= 0)
            throw new WorkflowDomainException("Due-after hours must be greater than zero.");

        Type = type;
        IsRequired = isRequired;
        AllowReassignment = allowReassignment;
        DueAfterHours = dueAfterHours;
        RequiredRole = NormalizeOptional(requiredRole, 100);
    }

    internal void Update(
        int sequence,
        string code,
        string name,
        string? description,
        WorkflowStepType type,
        bool isRequired,
        bool allowReassignment,
        int? dueAfterHours,
        string? requiredRole)
    {
        if (sequence <= 0)
            throw new WorkflowDomainException("Step sequence must be greater than zero.");

        if (!Enum.IsDefined(type))
            throw new WorkflowDomainException("Workflow step type is invalid.");

        if (dueAfterHours is <= 0)
            throw new WorkflowDomainException("Due-after hours must be greater than zero.");

        Sequence = sequence;
        Code = NormalizeRequired(code, nameof(code), 50).ToUpperInvariant();
        ValidateCode(Code);
        Name = NormalizeRequired(name, nameof(name), 150);
        Description = NormalizeOptional(description, 1000);
        Type = type;
        IsRequired = isRequired;
        AllowReassignment = allowReassignment;
        DueAfterHours = dueAfterHours;
        RequiredRole = NormalizeOptional(requiredRole, 100);
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
                "Step code can contain only letters, numbers, hyphens and underscores.");
        }
    }
}
