using Microsoft.AspNetCore.Authorization;

namespace LIMS.Api.Authorization.Permissions;

public sealed class PermissionRequirement(string permission)
    : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
