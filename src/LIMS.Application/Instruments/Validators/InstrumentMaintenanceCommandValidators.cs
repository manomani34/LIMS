using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Common.Validation;
using LIMS.Application.Instruments.Commands;

namespace LIMS.Application.Instruments.Validators;

public sealed class ScheduleInstrumentMaintenanceCommandValidator
    : AbstractValidator<ScheduleInstrumentMaintenanceCommand>
{
    public ScheduleInstrumentMaintenanceCommandValidator()
    {
        RuleFor(x => x.InstrumentId)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.ScheduledByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ScheduledAt)
            .Must(x => x != default);

        RuleFor(x => x.Type)
            .Must(Enum.IsDefined);
    }
}

public sealed class CompleteInstrumentMaintenanceCommandValidator
    : AbstractValidator<CompleteInstrumentMaintenanceCommand>
{
    public CompleteInstrumentMaintenanceCommandValidator()
    {
        RuleFor(x => x.InstrumentId)
            .NotEmpty();

        RuleFor(x => x.MaintenanceId)
            .NotEmpty();

        RuleFor(x => x.CompletedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CompletionNotes)
            .MaximumLength(2000);
    }
}
