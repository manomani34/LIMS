using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.DTOs;

namespace LIMS.Application.Protocols.Queries;

public sealed record GetProtocolsByLaboratoryIdQuery(
    Guid LaboratoryId) : IQuery<IReadOnlyCollection<ProtocolDto>>;
