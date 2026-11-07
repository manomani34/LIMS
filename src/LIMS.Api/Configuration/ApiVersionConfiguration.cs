namespace LIMS.Api.Configuration;

public static class ApiVersionConfiguration
{
    public const string CurrentVersion = "v1";

    public static IServiceCollection AddLimsApiVersioning(
        this IServiceCollection services)
    {
        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        return services;
    }
}
