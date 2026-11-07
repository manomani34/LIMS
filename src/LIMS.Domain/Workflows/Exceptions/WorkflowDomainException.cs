using LIMS.Domain.Common;

namespace LIMS.Domain.Workflows.Exceptions;

public sealed class WorkflowDomainException : DomainException
{
    public WorkflowDomainException(string message)
        : base(message)
    {
    }
}
