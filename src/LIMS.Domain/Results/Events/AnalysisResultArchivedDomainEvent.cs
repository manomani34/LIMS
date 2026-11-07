using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record AnalysisResultArchivedDomainEvent(
    Guid ResultId,
    string ArchivedByUserId,
    DateTimeOffset ArchivedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
