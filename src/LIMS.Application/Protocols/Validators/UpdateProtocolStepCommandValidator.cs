using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Commands;
using LIMS.Domain.Protocols.Enums;

namespace LIMS.Application.Protocols.Validators;

public sealed class UpdateProtocolStepCommandValidator
    : AbstractValidator<UpdateProtocolStepCommand>
{
    public UpdateProtocolStepCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");

        RuleFor(x => x.StepId)
            .Must(x => x != Guid.Empty, "StepId is required.");

        RuleFor(x => x.Sequence)
            .Must(x => x > 0, "Sequence must be greater than zero.");

        RuleFor(x => x.Title)
            .NotEmpty("Title is required.")
            .MaximumLength(200, "Title cannot exceed 200 characters.");

        RuleFor(x => x.Instruction)
            .NotEmpty("Instruction is required.")
            .MaximumLength(4000, "Instruction cannot exceed 4000 characters.");

        RuleFor(x => x.Type)
            .Must(x => Enum.IsDefined(x), "Protocol step type is invalid.");

        RuleFor(x => x.TargetUnit)
            .MaximumLength(50, "TargetUnit cannot exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000, "Notes cannot exceed 1000 characters.");

        RuleFor(x => x.MinimumValue)
            .Must((command, value) =>
                !value.HasValue ||
                !command.MaximumValue.HasValue ||
                value.Value <= command.MaximumValue.Value,
                "MinimumValue cannot exceed MaximumValue.");

        RuleFor(x => x.TargetValue)
            .Must((command, value) =>
                !value.HasValue ||
                (!command.MinimumValue.HasValue ||
                 value.Value >= command.MinimumValue.Value) &&
                (!command.MaximumValue.HasValue ||
                 value.Value <= command.MaximumValue.Value),
                "TargetValue must be within the configured range.");
    }
}
