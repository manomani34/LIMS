using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class CreateExperimentCommandValidator
    : AbstractValidator<CreateExperimentCommand>
{
    public CreateExperimentCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");

        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("SampleId is required.");

        RuleFor(x => x.ExperimentCode)
            .NotEmpty()
            .WithMessage("ExperimentCode is required.")
            .MaximumLength(50)
            .WithMessage("ExperimentCode cannot exceed 50 characters.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty()
            .WithMessage("CreatedByUserId is required.")
            .MaximumLength(150)
            .WithMessage("CreatedByUserId cannot exceed 150 characters.");
    }
}
