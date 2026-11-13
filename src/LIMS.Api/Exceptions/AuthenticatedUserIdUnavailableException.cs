namespace LIMS.Api.Exceptions;

public sealed class AuthenticatedUserIdUnavailableException : Exception
{
    public AuthenticatedUserIdUnavailableException()
        : base("Authenticated user ID is unavailable.")
    {
    }
}
