using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class AssignExperimentCommandValidator
    : AbstractValidator<AssignExperimentCommand>
{
    public AssignExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .MaximumLength(150)
            .WithMessage("UserId cannot exceed 150 characters.");
    }
}
