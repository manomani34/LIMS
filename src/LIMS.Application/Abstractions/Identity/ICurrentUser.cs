namespace LIMS.Application.Abstractions.Identity;

public interface ICurrentUser
{
    string? UserId { get; }

    string? UserName { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }

    IReadOnlyCollection<string> Roles { get; }
}
