using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestReviewStartedDomainEvent(
    Guid RequestId,
    string ReviewerUserId,
    DateTime ReviewedAt,
    DateTime OccurredOn) : IDomainEvent;
