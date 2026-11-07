using LIMS.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Configuration;

public static class ApiBehaviorConfiguration
{
    public static IServiceCollection AddApiBehaviorConfiguration(
        this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .SelectMany(entry =>
                        entry.Value!.Errors.Select(error =>
                            new ValidationError(
                                entry.Key,
                                string.IsNullOrWhiteSpace(error.ErrorMessage)
                                    ? "The supplied value is invalid."
                                    : error.ErrorMessage)))
                    .ToArray();

                var response = new ValidationErrorResponse(
                    Code: "VALIDATION_ERROR",
                    Message: "One or more validation errors occurred.",
                    TraceId: context.HttpContext.TraceIdentifier,
                    Errors: errors);

                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }
}
