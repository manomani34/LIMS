using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Protocols.Commands;

public sealed record ActivateProtocolCommand(
    Guid ProtocolId) : ICommand<bool>;
