using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LIMS.Api.Configuration;

public static class ApiResponseConfiguration
{
    public static IServiceCollection AddApiResponseConfiguration(
        this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressMapClientErrors = true;
            options.SuppressModelStateInvalidFilter = false;
        });

        services.AddSingleton<
            ProblemDetailsFactory,
            LimsProblemDetailsFactory>();

        return services;
    }
}
