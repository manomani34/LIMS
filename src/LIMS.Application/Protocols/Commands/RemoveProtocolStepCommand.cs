using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Protocols.Commands;

public sealed record RemoveProtocolStepCommand(
    Guid ProtocolId,
    Guid StepId) : ICommand<bool>;
