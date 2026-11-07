using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record AnalysisResultReviewStartedDomainEvent(
    Guid ResultId,
    string ReviewerUserId,
    DateTimeOffset StartedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
