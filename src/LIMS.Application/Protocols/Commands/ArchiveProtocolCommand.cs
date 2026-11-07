using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Protocols.Commands;

public sealed record ArchiveProtocolCommand(
    Guid ProtocolId) : ICommand<bool>;
