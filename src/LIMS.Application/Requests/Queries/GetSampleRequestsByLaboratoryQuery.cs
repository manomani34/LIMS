using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.DTOs;

namespace LIMS.Application.Requests.Queries;

public sealed record GetSampleRequestsByLaboratoryQuery(
    Guid LaboratoryId)
    : IQuery<IReadOnlyCollection<SampleRequestDto>>;
