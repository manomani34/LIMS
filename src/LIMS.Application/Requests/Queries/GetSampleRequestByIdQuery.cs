using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.DTOs;

namespace LIMS.Application.Requests.Queries;

public sealed record GetSampleRequestByIdQuery(
    Guid RequestId) : IQuery<SampleRequestDto?>;
