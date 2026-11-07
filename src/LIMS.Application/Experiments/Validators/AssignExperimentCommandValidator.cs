using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class AssignExperimentCommandValidator
    : AbstractValidator<AssignExperimentCommand>
{
    public AssignExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .Must(x => x != Guid.Empty, "ExperimentId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty("UserId is required.")
            .MaximumLength(150, "UserId cannot exceed 150 characters.");
    }
}
