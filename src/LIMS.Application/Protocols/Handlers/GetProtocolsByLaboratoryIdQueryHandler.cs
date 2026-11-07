using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Protocols.DTOs;
using LIMS.Application.Protocols.Queries;

namespace LIMS.Application.Protocols.Handlers;

public sealed class GetProtocolsByLaboratoryIdQueryHandler
    : IQueryHandler<
        GetProtocolsByLaboratoryIdQuery,
        IReadOnlyCollection<ProtocolDto>>
{
    private readonly IProtocolRepository _repository;

    public GetProtocolsByLaboratoryIdQueryHandler(
        IProtocolRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ProtocolDto>> HandleAsync(
        GetProtocolsByLaboratoryIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var protocols = await _repository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            cancellationToken);

        return protocols.Select(x => x.ToDto()).ToArray();
    }
}
