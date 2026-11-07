namespace LIMS.Domain.Workflows.Enums;

public enum WorkflowTaskStatus
{
    Pending = 1,
    Assigned = 2,
    InProgress = 3,
    Completed = 4,
    Rejected = 5,
    Skipped = 6,
    Cancelled = 7
}
