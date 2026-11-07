using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestSubmittedDomainEvent(
    Guid RequestId,
    string SubmittedByUserId,
    DateTime SubmittedAt,
    DateTime OccurredOn) : IDomainEvent;
