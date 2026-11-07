using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.DTOs;

namespace LIMS.Application.Samples.Queries;

public sealed record GetSamplesByAssignedUserIdQuery(
    string UserId) : IQuery<IReadOnlyCollection<SampleDto>>;
