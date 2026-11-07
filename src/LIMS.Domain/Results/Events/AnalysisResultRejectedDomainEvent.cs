using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record AnalysisResultRejectedDomainEvent(
    Guid ResultId,
    string RejectedByUserId,
    string Reason,
    DateTimeOffset RejectedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
