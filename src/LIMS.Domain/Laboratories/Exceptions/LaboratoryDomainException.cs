using LIMS.Domain.Common;

namespace LIMS.Domain.Laboratories.Exceptions;

public sealed class LaboratoryDomainException : DomainException
{
    public LaboratoryDomainException(string message)
        : base(message)
    {
    }
}
