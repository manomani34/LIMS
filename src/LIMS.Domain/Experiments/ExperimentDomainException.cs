using LIMS.Domain.Common;

namespace LIMS.Domain.Experiments;

public sealed class ExperimentDomainException : DomainException
{
    public ExperimentDomainException(string message) : base(message)
    {
    }
}
