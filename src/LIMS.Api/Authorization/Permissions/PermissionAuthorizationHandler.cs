using LIMS.Application.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace LIMS.Api.Authorization.Permissions;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionChecker _permissionChecker;

    public PermissionAuthorizationHandler(
        IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission =
            await _permissionChecker.HasPermissionAsync(
                requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
