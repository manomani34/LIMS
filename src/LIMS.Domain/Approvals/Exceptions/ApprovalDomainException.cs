using LIMS.Domain.Common;

namespace LIMS.Domain.Approvals.Exceptions;

public sealed class ApprovalDomainException : DomainException
{
    public ApprovalDomainException(string message)
        : base(message)
    {
    }
}
