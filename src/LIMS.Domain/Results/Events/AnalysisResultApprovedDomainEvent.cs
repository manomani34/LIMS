using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record AnalysisResultApprovedDomainEvent(
    Guid ResultId,
    string ApprovedByUserId,
    string? Comments,
    DateTimeOffset ApprovedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
