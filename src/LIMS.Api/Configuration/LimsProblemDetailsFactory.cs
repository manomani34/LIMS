
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LIMS.Api.Configuration;

public sealed class LimsProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode ?? StatusCodes.Status500InternalServerError,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        AddExtensions(problem, httpContext);

        return problem;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problem = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode ?? StatusCodes.Status400BadRequest,
            Title = title ?? "One or more validation errors occurred.",
            Type = type,
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        AddExtensions(problem, httpContext);

        return problem;
    }

    private static void AddExtensions(
        ProblemDetails problem,
        HttpContext httpContext)
    {
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (httpContext.Items.TryGetValue(
                Middleware.CorrelationIdMiddleware.ItemKey,
                out var correlationId))
        {
            problem.Extensions["correlationId"] = correlationId;
        }
    }
}
