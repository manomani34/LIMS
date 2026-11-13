using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Validators;

public sealed class UpdateProtocolStepCommandValidator
    : AbstractValidator<UpdateProtocolStepCommand>
{
    public UpdateProtocolStepCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .NotEmpty()
            .WithMessage("ProtocolId is required.");

        RuleFor(x => x.StepId)
            .NotEmpty()
            .WithMessage("StepId is required.");

        RuleFor(x => x.Sequence)
            .Must(x => x > 0)
            .WithMessage("Sequence must be greater than zero.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Instruction)
            .NotEmpty()
            .WithMessage("Instruction is required.")
            .MaximumLength(4000)
            .WithMessage("Instruction cannot exceed 4000 characters.");

        RuleFor(x => x.Type)
            .Must(x => Enum.IsDefined(x))
            .WithMessage("Protocol step type is invalid.");

        RuleFor(x => x.TargetUnit)
            .MaximumLength(50)
            .WithMessage("TargetUnit cannot exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.");

        RuleFor(x => x.MinimumValue)
            .Must((command, value) =>
                !value.HasValue ||
                !command.MaximumValue.HasValue ||
                value.Value <= command.MaximumValue.Value)
            .WithMessage("MinimumValue cannot exceed MaximumValue.");

        RuleFor(x => x.TargetValue)
            .Must((command, value) =>
                !value.HasValue ||
                ((!command.MinimumValue.HasValue ||
                  value.Value >= command.MinimumValue.Value) &&
                 (!command.MaximumValue.HasValue ||
                  value.Value <= command.MaximumValue.Value)))
            .WithMessage("TargetValue must be within the configured range.");
    }
}
