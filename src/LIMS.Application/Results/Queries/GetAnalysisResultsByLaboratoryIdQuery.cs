using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Queries;

public sealed record GetAnalysisResultsByLaboratoryIdQuery(
    Guid LaboratoryId) : IQuery<IReadOnlyCollection<DTOs.AnalysisResultDto>>;

