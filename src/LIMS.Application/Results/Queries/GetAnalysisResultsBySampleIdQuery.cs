using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsBySampleIdQuery(
    Guid SampleId) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;

