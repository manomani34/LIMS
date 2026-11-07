using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Validators;

public sealed class CreateExperimentCommandValidator
    : AbstractValidator<CreateExperimentCommand>
{
    public CreateExperimentCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty, "LaboratoryId is required.");

        RuleFor(x => x.SampleId)
            .Must(x => x != Guid.Empty, "SampleId is required.");

        RuleFor(x => x.ExperimentCode)
            .NotEmpty("ExperimentCode is required.")
            .MaximumLength(50, "ExperimentCode cannot exceed 50 characters.");

        RuleFor(x => x.Title)
            .NotEmpty("Title is required.")
            .MaximumLength(200, "Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000, "Description cannot exceed 2000 characters.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty("CreatedByUserId is required.")
            .MaximumLength(150, "CreatedByUserId cannot exceed 150 characters.");
    }
}
