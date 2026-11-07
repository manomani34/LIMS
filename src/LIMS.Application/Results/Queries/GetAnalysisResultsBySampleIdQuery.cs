using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsBySampleIdQuery(
    Guid SampleId) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;
