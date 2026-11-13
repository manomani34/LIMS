
using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Instruments.Commands;

namespace LIMS.Application.Instruments.Validators;

public sealed class RegisterInstrumentCommandValidator
    : AbstractValidator<RegisterInstrumentCommand>
{
    public RegisterInstrumentCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty()
            .WithMessage("LaboratoryId is required.");

        RuleFor(x => x.InstrumentCode)
            .NotEmpty()
            .WithMessage("InstrumentCode is required.")
            .MaximumLength(50)
            .WithMessage("InstrumentCode cannot exceed 50 characters.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Type)
            .Must(Enum.IsDefined)
            .WithMessage("Instrument type is invalid.");

        RuleFor(x => x.Manufacturer)
            .MaximumLength(200)
            .WithMessage("Manufacturer cannot exceed 200 characters.");

        RuleFor(x => x.Model)
            .MaximumLength(100)
            .WithMessage("Model cannot exceed 100 characters.");

        RuleFor(x => x.SerialNumber)
            .MaximumLength(100)
            .WithMessage("SerialNumber cannot exceed 100 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(300)
            .WithMessage("Location cannot exceed 300 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");
    }
}
