namespace LIMS.Api.Configuration;

public static class HealthCheckConfiguration
{
    public static IServiceCollection AddLimsHealthChecks(
        this IServiceCollection services)
    {
        services.AddHealthChecks();

        return services;
    }

    public static WebApplication MapLimsHealthChecks(
        this WebApplication app)
    {
        app.MapHealthChecks("/health")
            .AllowAnonymous();

        return app;
    }
}
