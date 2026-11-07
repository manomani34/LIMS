using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Results.DTOs;
using LIMS.Application.Results.Mapping;
using LIMS.Application.Results.Queries;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers.Queries;

public sealed class GetAnalysisResultByIdQueryHandler
    : IQueryHandler<GetAnalysisResultByIdQuery, AnalysisResultDetailsDto?>
{
    private readonly IAnalysisResultRepository _repository;

    public GetAnalysisResultByIdQueryHandler(
        IAnalysisResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnalysisResultDetailsDto?> HandleAsync(
        GetAnalysisResultByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetWithDetailsAsync(
            query.ResultId,
            cancellationToken);

        return result is null
            ? null
            : AnalysisResultMapping.ToDetailsDto(result);
    }
}
