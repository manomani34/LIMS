using LIMS.Application.Results.DTOs;
using LIMS.Application.Results.Mapping;

namespace LIMS.Application.Results.Handlers.Queries;

public sealed class GetAnalysisResultByCodeQueryHandler
    : IQueryHandler<GetAnalysisResultByCodeQuery, AnalysisResultDto?>
{
    private readonly IAnalysisResultRepository _repository;

    public GetAnalysisResultByCodeQueryHandler(
        IAnalysisResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnalysisResultDto?> HandleAsync(
        GetAnalysisResultByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetByCodeAsync(
            query.LaboratoryId,
            query.ResultCode,
            cancellationToken);

        return result is null
            ? null
            : AnalysisResultMapping.ToDto(result);
    }
}
