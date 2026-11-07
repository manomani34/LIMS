using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsByLaboratoryIdQuery(
    Guid LaboratoryId) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;
