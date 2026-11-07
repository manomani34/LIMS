using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.DTOs;

namespace LIMS.Application.Protocols.Queries;

public sealed record GetProtocolByCodeQuery(
    Guid LaboratoryId,
    string ProtocolCode) : IQuery<ProtocolDto?>;
