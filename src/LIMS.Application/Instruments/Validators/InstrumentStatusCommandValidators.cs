using LIMS.Application.Common.Validation;
using LIMS.Application.Instruments.Commands;

namespace LIMS.Application.Instruments.Validators;

public sealed class MarkInstrumentInUseCommandValidator
    : AbstractValidator<MarkInstrumentInUseCommand>
{
    public MarkInstrumentInUseCommandValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();
    }
}

public sealed class MarkInstrumentAvailableCommandValidator
    : AbstractValidator<MarkInstrumentAvailableCommand>
{
    public MarkInstrumentAvailableCommandValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();
    }
}

public sealed class MarkInstrumentOutOfServiceCommandValidator
    : AbstractValidator<MarkInstrumentOutOfServiceCommand>
{
    public MarkInstrumentOutOfServiceCommandValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(1000);
    }
}

public sealed class RestoreInstrumentServiceCommandValidator
    : AbstractValidator<RestoreInstrumentServiceCommand>
{
    public RestoreInstrumentServiceCommandValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();
    }
}

public sealed class RetireInstrumentCommandValidator
    : AbstractValidator<RetireInstrumentCommand>
{
    public RetireInstrumentCommandValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();
    }
}
