using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Results.DTOs;
using LIMS.Application.Results.Mapping;
using LIMS.Application.Results.Queries;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers.Queries;

public sealed class GetAnalysisResultsBySampleIdQueryHandler
    : IQueryHandler<
        GetAnalysisResultsBySampleIdQuery,
        IReadOnlyCollection<AnalysisResultDto>>
{
    private readonly IAnalysisResultRepository _repository;

    public GetAnalysisResultsBySampleIdQueryHandler(
        IAnalysisResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<AnalysisResultDto>> HandleAsync(
        GetAnalysisResultsBySampleIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var results = await _repository.GetBySampleIdAsync(
            query.SampleId,
            cancellationToken);

        return results
            .Select(AnalysisResultMapping.ToDto)
            .ToArray();
    }
}
