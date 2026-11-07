using LIMS.Domain.Common;

namespace LIMS.Domain.Instruments;

public sealed class InstrumentDomainException : DomainException
{
    public InstrumentDomainException(string message)
        : base(message)
    {
    }
}
