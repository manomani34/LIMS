using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowInstanceStartedDomainEvent(
    Guid WorkflowInstanceId,
    Guid WorkflowDefinitionId,
    string StartedByUserId,
    DateTime StartedAt,
    DateTime OccurredOn) : IDomainEvent;
