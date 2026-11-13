using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Events;

public sealed record WorkflowTaskCompletedDomainEvent(
    Guid WorkflowInstanceId,
    Guid TaskId,
    string CompletedByUserId,
    DateTime CompletedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

