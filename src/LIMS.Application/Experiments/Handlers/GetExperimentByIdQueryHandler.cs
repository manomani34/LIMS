using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.DTOs;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Handlers;

public sealed class GetExperimentByIdQueryHandler
    : IQueryHandler<GetExperimentByIdQuery, ExperimentDetailsDto?>
{
    private readonly IExperimentRepository _repository;

    public GetExperimentByIdQueryHandler(IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExperimentDetailsDto?> HandleAsync(
        GetExperimentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var experiment = await _repository.GetWithDetailsAsync(
            query.ExperimentId,
            cancellationToken);

        return experiment?.ToDetailsDto();
    }
}
