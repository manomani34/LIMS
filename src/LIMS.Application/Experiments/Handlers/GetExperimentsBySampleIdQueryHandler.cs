using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.DTOs;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Handlers;

public sealed class GetExperimentsBySampleIdQueryHandler
    : IQueryHandler<
        GetExperimentsBySampleIdQuery,
        IReadOnlyCollection<ExperimentDto>>
{
    private readonly IExperimentRepository _repository;

    public GetExperimentsBySampleIdQueryHandler(
        IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ExperimentDto>> HandleAsync(
        GetExperimentsBySampleIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var experiments = await _repository.GetBySampleIdAsync(
            query.SampleId,
            cancellationToken);

        return experiments.Select(x => x.ToDto()).ToArray();
    }
}
