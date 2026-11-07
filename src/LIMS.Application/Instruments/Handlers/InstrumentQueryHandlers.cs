using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Instruments.Abstractions;
using LIMS.Application.Instruments.DTOs;
using LIMS.Application.Instruments.Mapping;

namespace LIMS.Application.Instruments.Handlers;

public sealed class GetInstrumentByIdQueryHandler
    : IQueryHandler<Queries.GetInstrumentByIdQuery, InstrumentDetailsDto?>
{
    private readonly IInstrumentRepository _repository;

    public GetInstrumentByIdQueryHandler(IInstrumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<InstrumentDetailsDto?> HandleAsync(
        Queries.GetInstrumentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(query.InstrumentId, cancellationToken);
        return instrument?.ToDetailsDto();
    }
}

public sealed class GetInstrumentByCodeQueryHandler
    : IQueryHandler<Queries.GetInstrumentByCodeQuery, InstrumentDto?>
{
    private readonly IInstrumentRepository _repository;

    public GetInstrumentByCodeQueryHandler(IInstrumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<InstrumentDto?> HandleAsync(
        Queries.GetInstrumentByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetByCodeAsync(
            query.LaboratoryId,
            query.InstrumentCode,
            cancellationToken);

        return instrument?.ToDto();
    }
}

public sealed class GetInstrumentsByLaboratoryIdQueryHandler
    : IQueryHandler<Queries.GetInstrumentsByLaboratoryIdQuery, IReadOnlyCollection<InstrumentDto>>
{
    private readonly IInstrumentRepository _repository;

    public GetInstrumentsByLaboratoryIdQueryHandler(IInstrumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<InstrumentDto>> HandleAsync(
        Queries.GetInstrumentsByLaboratoryIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var instruments = await _repository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            cancellationToken);

        return instruments.Select(x => x.ToDto()).ToArray();
    }
}

public sealed class GetInstrumentsByStatusQueryHandler
    : IQueryHandler<Queries.GetInstrumentsByStatusQuery, IReadOnlyCollection<InstrumentDto>>
{
    private readonly IInstrumentRepository _repository;

    public GetInstrumentsByStatusQueryHandler(IInstrumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<InstrumentDto>> HandleAsync(
        Queries.GetInstrumentsByStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var instruments = await _repository.GetByStatusAsync(
            query.LaboratoryId,
            query.Status,
            cancellationToken);

        return instruments.Select(x => x.ToDto()).ToArray();
    }
}
