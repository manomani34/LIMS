using LIMS.Api.Authorization.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace LIMS.Api.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddLimsAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy =
                new AuthorizationPolicyBuilder(
                        JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
        });

        services.AddSingleton<
            IAuthorizationPolicyProvider,
            PermissionPolicyProvider>();

        services.AddScoped<
            IAuthorizationHandler,
            PermissionAuthorizationHandler>();

        return services;
    }
}
