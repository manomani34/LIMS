using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Protocols.Commands;

public sealed record CreateProtocolCommand(
    Guid LaboratoryId,
    string ProtocolCode,
    string Name,
    string? Description,
    string? Version,
    string CreatedByUserId) : ICommand<Guid>;
