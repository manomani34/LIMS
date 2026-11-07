using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Protocols.Commands;

public sealed record DeprecateProtocolCommand(
    Guid ProtocolId) : ICommand<bool>;
