using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Samples.Commands;

namespace LIMS.Application.Samples.Validators;

public sealed class RegisterSampleCommandValidator
    : AbstractValidator<RegisterSampleCommand>
{
    public RegisterSampleCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("Laboratory identifier is required.");

        RuleFor(x => x.SampleCode)
            .NotEmpty()
            .WithMessage("Sample code is required.")
            .MaximumLength(50)
            .WithMessage("Sample code cannot exceed 50 characters.");

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

        RuleFor(x => x.RegisteredByUserId)
            .NotEmpty()
            .WithMessage("Registering user identifier is required.")
            .MaximumLength(150)
            .WithMessage("Registering user identifier cannot exceed 150 characters.");

        RuleFor(x => x.Type)
            .Must(Enum.IsDefined)
            .WithMessage("Sample type is invalid.");

        RuleFor(x => x.Priority)
            .Must(Enum.IsDefined)
            .WithMessage("Sample priority is invalid.");
    }
}
