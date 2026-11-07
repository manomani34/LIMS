using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsByExperimentIdQuery(
    Guid ExperimentId) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;
