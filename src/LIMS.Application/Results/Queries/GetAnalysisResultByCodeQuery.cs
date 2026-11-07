using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultByCodeQuery(
    Guid LaboratoryId,
    string ResultCode) : IQuery<DTOs.AnalysisResultDto?>;
