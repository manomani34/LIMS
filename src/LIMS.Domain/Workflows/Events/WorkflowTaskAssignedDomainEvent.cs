using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowTaskAssignedDomainEvent(
    Guid WorkflowInstanceId,
    Guid TaskId,
    string AssignedToUserId,
    DateTime AssignedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

