using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record AnalysisResultSubmittedDomainEvent(
    Guid ResultId,
    string SubmittedByUserId,
    DateTimeOffset SubmittedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
