using LIMS.Domain.Common;

namespace LIMS.Domain.Results;

public sealed class AnalysisResultDomainException : DomainException
{
    public AnalysisResultDomainException(string message)
        : base(message)
    {
    }
}
