using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.DTOs;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Handlers;

public sealed class GetExperimentsByLaboratoryIdQueryHandler
    : IQueryHandler<
        GetExperimentsByLaboratoryIdQuery,
        IReadOnlyCollection<ExperimentDto>>
{
    private readonly IExperimentRepository _repository;

    public GetExperimentsByLaboratoryIdQueryHandler(
        IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ExperimentDto>> HandleAsync(
        GetExperimentsByLaboratoryIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var experiments = await _repository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            cancellationToken);

        return experiments.Select(x => x.ToDto()).ToArray();
    }
}
