using LIMS.Domain.Common;

namespace LIMS.Domain.Samples.Exceptions;

public sealed class SampleDomainException : DomainException
{
    public SampleDomainException(string message)
        : base(message)
    {
    }
}
