using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class UpdateExperimentInformationCommandValidator
    : AbstractValidator<UpdateExperimentInformationCommand>
{
    public UpdateExperimentInformationCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .NotEmpty()
            .WithMessage("ExperimentId is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");
    }
}
