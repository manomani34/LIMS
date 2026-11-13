using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class UpdateSampleInformationCommandValidator
    : AbstractValidator<UpdateSampleInformationCommand>
{
    public UpdateSampleInformationCommandValidator()
    {
        RuleFor(x => x.SampleId)
            .NotEmpty()
            .WithMessage("Sample identifier is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Sample name is required.")
            .MaximumLength(200)
            .WithMessage("Sample name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Source)
            .MaximumLength(250)
            .WithMessage("Source cannot exceed 250 characters.");
    }
}
