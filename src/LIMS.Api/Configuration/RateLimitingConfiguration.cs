using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace LIMS.Api.Configuration;

public static class RateLimitingConfiguration
{
    public const string DefaultPolicy = "default";

    public static IServiceCollection AddLimsRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(DefaultPolicy, limiter =>
            {
                limiter.PermitLimit = 100;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueLimit = 0;
                limiter.AutoReplenishment = true;
            });
        });

        return services;
    }
}
