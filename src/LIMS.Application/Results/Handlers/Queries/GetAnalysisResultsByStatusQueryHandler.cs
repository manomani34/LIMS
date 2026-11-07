using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Results.DTOs;
using LIMS.Application.Results.Mapping;
using LIMS.Application.Results.Queries;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers.Queries;

public sealed class GetAnalysisResultsByStatusQueryHandler
    : IQueryHandler<
        GetAnalysisResultsByStatusQuery,
        IReadOnlyCollection<AnalysisResultDto>>
{
    private readonly IAnalysisResultRepository _repository;

    public GetAnalysisResultsByStatusQueryHandler(
        IAnalysisResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<AnalysisResultDto>> HandleAsync(
        GetAnalysisResultsByStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var results = await _repository.GetByStatusAsync(
            query.LaboratoryId,
            query.Status,
            cancellationToken);

        return results
            .Select(AnalysisResultMapping.ToDto)
            .ToArray();
    }
}
