using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class CancelExperimentCommandValidator
    : AbstractValidator<CancelExperimentCommand>
{
    public CancelExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .Must(x => x != Guid.Empty, "ExperimentId is required.");

        RuleFor(x => x.Reason)
            .NotEmpty("Cancellation reason is required.")
            .MaximumLength(1000, "Cancellation reason cannot exceed 1000 characters.");
    }
}
