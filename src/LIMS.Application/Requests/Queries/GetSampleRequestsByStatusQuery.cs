using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.DTOs;
using LIMS.Domain.Requests.Enums;

namespace LIMS.Application.Requests.Queries;

public sealed record GetSampleRequestsByStatusQuery(
    SampleRequestStatus Status)
    : IQuery<IReadOnlyCollection<SampleRequestDto>>;
