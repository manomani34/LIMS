using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Results.Commands;
using LIMS.Application.Abstractions.Validation;
using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.Validators;

public sealed class UpdateResultEntryCommandValidator
    : AbstractValidator<UpdateResultEntryCommand>
{
    public UpdateResultEntryCommandValidator()
    {
        RuleFor(x => x.ResultId)
            .NotEmpty();

        RuleFor(x => x.EntryId)
            .NotEmpty();

        RuleFor(x => x.ParameterName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ValueType)
            .Must(Enum.IsDefined)
            .WithMessage("Invalid result value type.");

        RuleFor(x => x.Value)
            .MaximumLength(4000);

        RuleFor(x => x.Unit)
            .MaximumLength(50);

        RuleFor(x => x.Uncertainty)
            .Must(x => !x.HasValue || x.Value >= 0)
            .WithMessage("Uncertainty cannot be negative.");

        RuleFor(x => x.Method)
            .MaximumLength(200);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);

        RuleFor(x => x)
            .Must(x => IsValueConsistent(
                x.ValueType,
                x.Value,
                x.NumericValue))
            .WithMessage("The supplied value does not match the selected value type.");
    }

    private static bool IsValueConsistent(
        ResultValueType valueType,
        string? value,
        decimal? numericValue)
    {
        return valueType switch
        {
            ResultValueType.Numeric => numericValue.HasValue,
            ResultValueType.Text => !string.IsNullOrWhiteSpace(value),
            ResultValueType.Boolean =>
                string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "false", StringComparison.OrdinalIgnoreCase),
            ResultValueType.Date =>
                DateOnly.TryParse(value, out _),
            _ => false
        };
    }
}
