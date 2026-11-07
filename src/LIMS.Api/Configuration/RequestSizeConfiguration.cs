namespace LIMS.Api.Configuration;

public static class RequestSizeConfiguration
{
    public const long DefaultMaximumRequestBodySize = 10 * 1024 * 1024;

    public static IServiceCollection AddLimsRequestSizeLimits(
        this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(
            options =>
            {
                options.MultipartBodyLengthLimit =
                    DefaultMaximumRequestBodySize;
            });

        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(
            options =>
            {
                options.Limits.MaxRequestBodySize =
                    DefaultMaximumRequestBodySize;
            });

        return services;
    }
}
