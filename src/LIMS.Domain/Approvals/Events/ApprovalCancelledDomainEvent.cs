using LIMS.Domain.Common;

namespace LIMS.Domain.Approvals.Events;

public sealed record ApprovalCancelledDomainEvent(
    Guid ApprovalId,
    string CancelledByUserId,
    string? Reason,
    DateTime CancelledAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

