using LIMS.Application.Common.Validation;
using LIMS.Application.Instruments.Commands;

namespace LIMS.Application.Instruments.Validators;

public sealed class RegisterInstrumentCommandValidator
    : AbstractValidator<RegisterInstrumentCommand>
{
    public RegisterInstrumentCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .NotEmpty();

        RuleFor(x => x.InstrumentCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.RegisteredByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Type)
            .Must(Enum.IsDefined);
    }
}
