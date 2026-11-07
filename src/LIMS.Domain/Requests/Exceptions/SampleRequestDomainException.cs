using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Exceptions;

public sealed class SampleRequestDomainException : DomainException
{
    public SampleRequestDomainException(string message)
        : base(message)
    {
    }
}
