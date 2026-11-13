using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class AssignSampleCommandValidator
    : AbstractValidator<AssignSampleCommand>
{
    public AssignSampleCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User identifier is required.")
            .MaximumLength(150)
            .WithMessage("User identifier cannot exceed 150 characters.");
    }
}
