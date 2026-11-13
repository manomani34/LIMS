using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestCancelledDomainEvent(
    Guid RequestId,
    string CancelledByUserId,
    string? Reason,
    DateTime CancelledAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

