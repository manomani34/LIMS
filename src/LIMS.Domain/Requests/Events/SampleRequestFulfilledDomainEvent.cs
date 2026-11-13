using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestFulfilledDomainEvent(
    Guid RequestId,
    Guid SampleId,
    DateTime FulfilledAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

