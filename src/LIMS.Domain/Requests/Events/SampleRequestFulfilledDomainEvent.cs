using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestFulfilledDomainEvent(
    Guid RequestId,
    Guid SampleId,
    DateTime FulfilledAt,
    DateTime OccurredOn) : IDomainEvent;
