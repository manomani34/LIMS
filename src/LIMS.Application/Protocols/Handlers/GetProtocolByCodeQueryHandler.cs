using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Protocols.DTOs;
using LIMS.Application.Protocols.Queries;

namespace LIMS.Application.Protocols.Handlers;

public sealed class GetProtocolByCodeQueryHandler
    : IQueryHandler<GetProtocolByCodeQuery, ProtocolDto?>
{
    private readonly IProtocolRepository _repository;

    public GetProtocolByCodeQueryHandler(IProtocolRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProtocolDto?> HandleAsync(
        GetProtocolByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var protocol = await _repository.GetByCodeAsync(
            query.LaboratoryId,
            query.ProtocolCode,
            cancellationToken);

        return protocol?.ToDto();
    }
}
