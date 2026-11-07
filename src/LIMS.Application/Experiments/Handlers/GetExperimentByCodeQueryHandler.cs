using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.DTOs;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Handlers;

public sealed class GetExperimentByCodeQueryHandler
    : IQueryHandler<GetExperimentByCodeQuery, ExperimentDto?>
{
    private readonly IExperimentRepository _repository;

    public GetExperimentByCodeQueryHandler(IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExperimentDto?> HandleAsync(
        GetExperimentByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var experiment = await _repository.GetByCodeAsync(
            query.LaboratoryId,
            query.ExperimentCode,
            cancellationToken);

        return experiment?.ToDto();
    }
}
