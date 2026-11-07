using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowInstanceCancelledDomainEvent(
    Guid WorkflowInstanceId,
    string CancelledByUserId,
    string? Reason,
    DateTime CancelledAt,
    DateTime OccurredOn) : IDomainEvent;
