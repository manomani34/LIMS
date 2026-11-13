using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestRejectedDomainEvent(
    Guid RequestId,
    string RejectedByUserId,
    string Reason,
    DateTime RejectedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

