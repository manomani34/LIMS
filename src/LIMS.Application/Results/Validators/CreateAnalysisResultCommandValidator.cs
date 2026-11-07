using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Results.Validators;

public sealed class CreateAnalysisResultCommandValidator
    : AbstractValidator<CreateAnalysisResultCommand>
{
    public CreateAnalysisResultCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty();

        RuleFor(x => x.ExperimentId)
            .NotEmpty();

        RuleFor(x => x.SampleId)
            .NotEmpty();

        RuleFor(x => x.InstrumentId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("InstrumentId must be a valid identifier when provided.");

        RuleFor(x => x.ResultCode)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Z0-9_-]+$")
            .WithMessage("Result code must contain only uppercase letters, numbers, hyphens or underscores.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty()
            .MaximumLength(150);
    }
}
