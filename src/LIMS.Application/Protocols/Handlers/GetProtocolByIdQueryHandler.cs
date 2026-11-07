using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Protocols.DTOs;
using LIMS.Application.Protocols.Queries;

namespace LIMS.Application.Protocols.Handlers;

public sealed class GetProtocolByIdQueryHandler
    : IQueryHandler<GetProtocolByIdQuery, ProtocolDetailsDto?>
{
    private readonly IProtocolRepository _repository;

    public GetProtocolByIdQueryHandler(IProtocolRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProtocolDetailsDto?> HandleAsync(
        GetProtocolByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var protocol = await _repository.GetWithDetailsAsync(
            query.ProtocolId,
            cancellationToken);

        return protocol?.ToDetailsDto();
    }
}
