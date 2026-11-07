using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowTaskRejectedDomainEvent(
    Guid WorkflowInstanceId,
    Guid TaskId,
    string RejectedByUserId,
    string Reason,
    DateTime RejectedAt,
    DateTime OccurredOn) : IDomainEvent;
