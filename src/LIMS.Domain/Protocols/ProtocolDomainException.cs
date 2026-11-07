using LIMS.Domain.Common;

namespace LIMS.Domain.Protocols;

public sealed class ProtocolDomainException : DomainException
{
    public ProtocolDomainException(string message)
        : base(message)
    {
    }
}
