namespace LIMS.Domain.Workflows.Enums;

public enum WorkflowInstanceStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Rejected = 4,
    Cancelled = 5,
    Suspended = 6
}
