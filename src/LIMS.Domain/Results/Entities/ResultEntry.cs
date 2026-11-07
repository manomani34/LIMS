using LIMS.Domain.Common;
using LIMS.Domain.Results.Enums;

namespace LIMS.Domain.Results;

public sealed class ResultEntry : Entity<Guid>
{
    public Guid ResultId { get; private set; }
    public string ParameterName { get; private set; } = string.Empty;
    public ResultValueType ValueType { get; private set; }
    public string? Value { get; private set; }
    public decimal? NumericValue { get; private set; }
    public string? Unit { get; private set; }
    public decimal? Uncertainty { get; private set; }
    public string? Method { get; private set; }
    public string? Notes { get; private set; }

    private ResultEntry()
    {
    }

    internal ResultEntry(
        Guid id,
        Guid resultId,
        string parameterName,
        ResultValueType valueType,
        string? value,
        decimal? numericValue,
        string? unit,
        decimal? uncertainty,
        string? method,
        string? notes) : base(id)
    {
        if (resultId == Guid.Empty)
            throw new AnalysisResultDomainException("ResultId is required.");

        ResultId = resultId;
        Apply(
            parameterName,
            valueType,
            value,
            numericValue,
            unit,
            uncertainty,
            method,
            notes);
    }

    internal void Update(
        string parameterName,
        ResultValueType valueType,
        string? value,
        decimal? numericValue,
        string? unit,
        decimal? uncertainty,
        string? method,
        string? notes)
    {
        Apply(
            parameterName,
            valueType,
            value,
            numericValue,
            unit,
            uncertainty,
            method,
            notes);
    }

    private void Apply(
        string parameterName,
        ResultValueType valueType,
        string? value,
        decimal? numericValue,
        string? unit,
        decimal? uncertainty,
        string? method,
        string? notes)
    {
        ParameterName = NormalizeRequired(
            parameterName,
            150,
            nameof(parameterName));

        if (!Enum.IsDefined(valueType))
            throw new AnalysisResultDomainException("Invalid result value type.");

        if (uncertainty < 0)
            throw new AnalysisResultDomainException(
                "Uncertainty cannot be negative.");

        ValueType = valueType;
        Value = NormalizeOptional(value, 4000);
        NumericValue = numericValue;
        Unit = NormalizeOptional(unit, 50);
        Uncertainty = uncertainty;
        Method = NormalizeOptional(method, 200);
        Notes = NormalizeOptional(notes, 2000);

        ValidateValue();
    }

    private void ValidateValue()
    {
        switch (ValueType)
        {
            case ResultValueType.Numeric:
                if (!NumericValue.HasValue)
                    throw new AnalysisResultDomainException(
                        "NumericValue is required for numeric results.");
                break;

            case ResultValueType.Text:
                if (string.IsNullOrWhiteSpace(Value))
                    throw new AnalysisResultDomainException(
                        "Value is required for text results.");

                if (NumericValue.HasValue)
                    throw new AnalysisResultDomainException(
                        "NumericValue is not allowed for text results.");
                break;

            case ResultValueType.Boolean:
                if (!string.Equals(Value, "true", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(Value, "false", StringComparison.OrdinalIgnoreCase))
                {
                    throw new AnalysisResultDomainException(
                        "Boolean values must be true or false.");
                }

                if (NumericValue.HasValue)
                    throw new AnalysisResultDomainException(
                        "NumericValue is not allowed for boolean results.");
                break;

            case ResultValueType.Date:
                if (!DateOnly.TryParse(Value, out _))
                    throw new AnalysisResultDomainException(
                        "A valid date value is required.");

                if (NumericValue.HasValue)
                    throw new AnalysisResultDomainException(
                        "NumericValue is not allowed for date results.");
                break;

            default:
                throw new AnalysisResultDomainException(
                    "Unsupported result value type.");
        }
    }

    private static string NormalizeRequired(
        string? value,
        int maxLength,
        string parameterName)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new AnalysisResultDomainException(
                $"{parameterName} is required.");

        if (normalized.Length > maxLength)
            throw new AnalysisResultDomainException(
                $"{parameterName} cannot exceed {maxLength} characters.");

        return normalized;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
            throw new AnalysisResultDomainException(
                $"Value cannot exceed {maxLength} characters.");

        return normalized;
    }
}
