using LIMS.Domain.Common;

namespace LIMS.Domain.Files;

public sealed class FileDomainException : DomainException
{
    public FileDomainException(string message)
        : base(message)
    {
    }
}
