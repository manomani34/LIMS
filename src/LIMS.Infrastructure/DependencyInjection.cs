using LIMS.Application.Abstractions.Identity;
using LIMS.Application.Abstractions.Security;
using LIMS.Application.Abstractions.Time;
using LIMS.Infrastructure.Identity.Authorization;
using LIMS.Infrastructure.Identity.Users;
using LIMS.Infrastructure.Time;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
