using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Results.DTOs;
using LIMS.Application.Results.Mapping;
using LIMS.Application.Results.Queries;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers.Queries;

public sealed class GetAnalysisResultsByExperimentIdQueryHandler
    : IQueryHandler<
        GetAnalysisResultsByExperimentIdQuery,
        IReadOnlyCollection<AnalysisResultDto>>
{
    private readonly IAnalysisResultRepository _repository;

    public GetAnalysisResultsByExperimentIdQueryHandler(
        IAnalysisResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<AnalysisResultDto>> HandleAsync(
        GetAnalysisResultsByExperimentIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var results = await _repository.GetByExperimentIdAsync(
            query.ExperimentId,
            cancellationToken);

        return results
            .Select(AnalysisResultMapping.ToDto)
            .ToArray();
    }
}
