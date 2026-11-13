using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowDefinitionCreatedDomainEvent(
    Guid WorkflowDefinitionId,
    string Code,
    string Name,
    DateTimeOffset OccurredOn) : IDomainEvent;

