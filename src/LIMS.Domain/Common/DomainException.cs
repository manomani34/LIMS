namespace LIMS.Domain.Common;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
