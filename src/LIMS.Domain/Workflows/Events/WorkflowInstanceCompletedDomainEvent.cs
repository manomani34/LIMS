using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowInstanceCompletedDomainEvent(
    Guid WorkflowInstanceId,
    DateTime CompletedAt,
    DateTime OccurredOn) : IDomainEvent;
