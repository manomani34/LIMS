using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.DTOs;
using LIMS.Application.Experiments.Queries;

namespace LIMS.Application.Experiments.Handlers;

public sealed class GetExperimentsByAssignedUserIdQueryHandler
    : IQueryHandler<
        GetExperimentsByAssignedUserIdQuery,
        IReadOnlyCollection<ExperimentDto>>
{
    private readonly IExperimentRepository _repository;

    public GetExperimentsByAssignedUserIdQueryHandler(
        IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ExperimentDto>> HandleAsync(
        GetExperimentsByAssignedUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var experiments = await _repository.GetByAssignedUserIdAsync(
            query.UserId,
            cancellationToken);

        return experiments.Select(x => x.ToDto()).ToArray();
    }
}
