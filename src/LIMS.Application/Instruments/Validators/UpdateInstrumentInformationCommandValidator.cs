using LIMS.Application.Common.Validation;
using LIMS.Application.Instruments.Commands;

namespace LIMS.Application.Instruments.Validators;

public sealed class UpdateInstrumentInformationCommandValidator
    : AbstractValidator<UpdateInstrumentInformationCommand>
{
    public UpdateInstrumentInformationCommandValidator()
    {
        RuleFor(x => x.InstrumentId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .Must(Enum.IsDefined);
    }
}
