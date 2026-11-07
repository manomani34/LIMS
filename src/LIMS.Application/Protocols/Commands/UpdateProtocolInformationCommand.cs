using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Protocols.Commands;

public sealed record UpdateProtocolInformationCommand(
    Guid ProtocolId,
    string Name,
    string? Description,
    string? Version) : ICommand<bool>;
