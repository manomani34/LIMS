using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultByIdQuery(
    Guid ResultId) : IQuery<DTOs.AnalysisResultDetailsDto?>;
