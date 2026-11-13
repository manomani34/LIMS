using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsByStatusQuery(
    Guid LaboratoryId,
    ResultStatus Status) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;

