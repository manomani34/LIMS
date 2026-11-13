using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultByCodeQuery(
    Guid LaboratoryId,
    string ResultCode) : IQuery<DTOs.AnalysisResultDto?>;

