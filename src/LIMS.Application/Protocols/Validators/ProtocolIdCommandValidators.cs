using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Validators;

public sealed class RemoveProtocolStepCommandValidator
    : AbstractValidator<RemoveProtocolStepCommand>
{
    public RemoveProtocolStepCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");

        RuleFor(x => x.StepId)
            .Must(x => x != Guid.Empty, "StepId is required.");
    }
}

public sealed class ActivateProtocolCommandValidator
    : AbstractValidator<ActivateProtocolCommand>
{
    public ActivateProtocolCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");
    }
}

public sealed class DeprecateProtocolCommandValidator
    : AbstractValidator<DeprecateProtocolCommand>
{
    public DeprecateProtocolCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");
    }
}

public sealed class ArchiveProtocolCommandValidator
    : AbstractValidator<ArchiveProtocolCommand>
{
    public ArchiveProtocolCommandValidator()
    {
        RuleFor(x => x.ProtocolId)
            .Must(x => x != Guid.Empty, "ProtocolId is required.");
    }
}
