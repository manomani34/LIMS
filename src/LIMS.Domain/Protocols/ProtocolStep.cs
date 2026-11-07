using LIMS.Domain.Common;
using LIMS.Domain.Protocols.Enums;

namespace LIMS.Domain.Protocols;

public sealed class ProtocolStep : Entity<Guid>
{
    public Guid ProtocolId { get; private set; }

    public int Sequence { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Instruction { get; private set; } = string.Empty;

    public ProtocolStepType Type { get; private set; }

    public bool IsRequired { get; private set; }

    public decimal? TargetValue { get; private set; }

    public string? TargetUnit { get; private set; }

    public decimal? MinimumValue { get; private set; }

    public decimal? MaximumValue { get; private set; }

    public string? Notes { get; private set; }

    private ProtocolStep()
    {
    }

    internal ProtocolStep(
        Guid protocolId,
        int sequence,
        string title,
        string instruction,
        ProtocolStepType type,
        bool isRequired,
        decimal? targetValue,
        string? targetUnit,
        decimal? minimumValue,
        decimal? maximumValue,
        string? notes)
        : base(Guid.NewGuid())
    {
        if (protocolId == Guid.Empty)
            throw new ProtocolDomainException("ProtocolId is required.");

        ProtocolId = protocolId;
        Update(
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
    }

    internal void Update(
        int sequence,
        string title,
        string instruction,
        ProtocolStepType type,
        bool isRequired,
        decimal? targetValue,
        string? targetUnit,
        decimal? minimumValue,
        decimal? maximumValue,
        string? notes)
    {
        if (sequence <= 0)
            throw new ProtocolDomainException("Step sequence must be greater than zero.");

        Title = NormalizeRequiredText(title, 200, "Step title");
        Instruction = NormalizeRequiredText(instruction, 4000, "Step instruction");

        if (!Enum.IsDefined(type))
            throw new ProtocolDomainException("Invalid protocol step type.");

        if (minimumValue.HasValue &&
            maximumValue.HasValue &&
            minimumValue.Value > maximumValue.Value)
        {
            throw new ProtocolDomainException(
                "Minimum value cannot be greater than maximum value.");
        }

        if (targetValue.HasValue &&
            minimumValue.HasValue &&
            targetValue.Value < minimumValue.Value)
        {
            throw new ProtocolDomainException(
                "Target value cannot be lower than minimum value.");
        }

        if (targetValue.HasValue &&
            maximumValue.HasValue &&
            targetValue.Value > maximumValue.Value)
        {
            throw new ProtocolDomainException(
                "Target value cannot be greater than maximum value.");
        }

        Sequence = sequence;
        Type = type;
        IsRequired = isRequired;
        TargetValue = targetValue;
        TargetUnit = NormalizeOptionalText(targetUnit, 50, "Target unit");
        MinimumValue = minimumValue;
        MaximumValue = maximumValue;
        Notes = NormalizeOptionalText(notes, 1000, "Step notes");
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
