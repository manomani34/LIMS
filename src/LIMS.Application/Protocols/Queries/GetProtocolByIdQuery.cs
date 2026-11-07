using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.DTOs;

namespace LIMS.Application.Protocols.Queries;

public sealed record GetProtocolByIdQuery(
    Guid ProtocolId) : IQuery<ProtocolDetailsDto?>;
