using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LIMS.Api.Authorization.Permissions;

public sealed class PermissionPolicyProvider
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider =
            new DefaultAuthorizationPolicyProvider(options);
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (!policyName.StartsWith(
                PermissionConstants.PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return await _fallbackPolicyProvider.GetPolicyAsync(
                policyName);
        }

        var permission = policyName[
            PermissionConstants.PolicyPrefix.Length..];

        if (string.IsNullOrWhiteSpace(permission))
            return null;

        return new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .AddRequirements(
                new PermissionRequirement(permission))
            .Build();
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}
