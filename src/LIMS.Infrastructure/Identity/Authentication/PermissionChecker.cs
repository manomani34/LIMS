using LIMS.Application.Abstractions.Identity;
using LIMS.Application.Abstractions.Security;

namespace LIMS.Infrastructure.Identity.Authorization;

public sealed class PermissionChecker : IPermissionChecker
{
    private readonly ICurrentUser _currentUser;

    public PermissionChecker(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public Task<bool> HasPermissionAsync(
        string permission,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        cancellationToken.ThrowIfCancellationRequested();

        var result = _currentUser.IsAuthenticated &&
                     _currentUser.Roles.Contains(
                         permission,
                         StringComparer.OrdinalIgnoreCase);

        return Task.FromResult(result);
    }
}
