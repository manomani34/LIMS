using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultByIdQuery(
    Guid ResultId) : IQuery<DTOs.AnalysisResultDetailsDto?>;

