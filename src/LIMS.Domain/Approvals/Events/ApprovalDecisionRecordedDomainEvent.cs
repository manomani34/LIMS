using LIMS.Domain.Common;
using LIMS.Domain.Approvals.Enums;

namespace LIMS.Domain.Approvals.Events;

public sealed record ApprovalDecisionRecordedDomainEvent(
    Guid ApprovalId,
    ApprovalDecision Decision,
    string DecidedByUserId,
    string? Comments,
    DateTime DecidedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

