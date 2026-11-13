using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsByExperimentIdQuery(
    Guid ExperimentId) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;

