using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowDefinitionActivatedDomainEvent(
    Guid WorkflowDefinitionId,
    DateTime ActivatedAt,
    DateTime OccurredOn) : IDomainEvent;
