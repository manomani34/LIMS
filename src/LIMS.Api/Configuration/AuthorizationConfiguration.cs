namespace LIMS.Api.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddLimsAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization();
        return services;
    }
}
