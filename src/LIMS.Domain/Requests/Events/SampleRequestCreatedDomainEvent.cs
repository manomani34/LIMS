using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestCreatedDomainEvent(
    Guid RequestId,
    string RequestCode,
    string RequestedByUserId,
    DateTimeOffset OccurredOn) : IDomainEvent;

