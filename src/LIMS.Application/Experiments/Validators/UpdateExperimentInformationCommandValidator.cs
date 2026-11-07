using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class UpdateExperimentInformationCommandValidator
    : AbstractValidator<UpdateExperimentInformationCommand>
{
    public UpdateExperimentInformationCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .Must(x => x != Guid.Empty, "ExperimentId is required.");

        RuleFor(x => x.Title)
            .NotEmpty("Title is required.")
            .MaximumLength(200, "Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000, "Description cannot exceed 2000 characters.");
    }
}
