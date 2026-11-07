using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Results.DTOs;
using LIMS.Application.Results.Mapping;
using LIMS.Application.Results.Queries;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers.Queries;

public sealed class GetAnalysisResultsByLaboratoryIdQueryHandler
    : IQueryHandler<
        GetAnalysisResultsByLaboratoryIdQuery,
        IReadOnlyCollection<AnalysisResultDto>>
{
    private readonly IAnalysisResultRepository _repository;

    public GetAnalysisResultsByLaboratoryIdQueryHandler(
        IAnalysisResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<AnalysisResultDto>> HandleAsync(
        GetAnalysisResultsByLaboratoryIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var results = await _repository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            cancellationToken);

        return results
            .Select(AnalysisResultMapping.ToDto)
            .ToArray();
    }
}
