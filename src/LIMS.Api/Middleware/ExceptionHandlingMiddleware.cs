using System.Text.Json;
using LIMS.Api.Exceptions;
using LIMS.Application.Abstractions.Validation;
using LIMS.Contracts.Common;
using LIMS.Domain.Common;

namespace LIMS.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Request cancelled. TraceId: {TraceId}",
                context.TraceIdentifier);
        }
        catch (AuthenticatedUserIdUnavailableException exception)
        {
            await HandleAuthenticatedUserIdUnavailableAsync(
                context,
                exception);
        }
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(context, exception);
        }
        catch (DomainException exception)
        {
            await HandleDomainExceptionAsync(context, exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}, Path: {Path}",
                context.TraceIdentifier,
                context.Request.Path);

            await HandleExceptionAsync(context);
        }
    }

    private async Task HandleAuthenticatedUserIdUnavailableAsync(
        HttpContext context,
        AuthenticatedUserIdUnavailableException exception)
    {
        if (context.Response.HasStarted)
            return;

        _logger.LogWarning(
            exception,
            "Authenticated user ID is unavailable. TraceId: {TraceId}, Path: {Path}",
            context.TraceIdentifier,
            context.Request.Path);

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            Code: "AUTHENTICATED_USER_ID_UNAVAILABLE",
            Message: "The authenticated user ID is unavailable.",
            TraceId: context.TraceIdentifier);

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            response,
            JsonOptions,
            context.RequestAborted);
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = exception.Errors
            .Select(x => new ValidationError(
                x.PropertyName,
                x.ErrorMessage,
                x.ErrorCode))
            .ToArray();

        var response = new ValidationErrorResponse(
            Code: "VALIDATION_ERROR",
            Message: "One or more validation errors occurred.",
            Errors: errors,
            TraceId: context.TraceIdentifier);

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            response,
            JsonOptions,
            context.RequestAborted);
    }

    private async Task HandleDomainExceptionAsync(
        HttpContext context,
        DomainException exception)
    {
        if (context.Response.HasStarted)
            return;

        _logger.LogWarning(
            exception,
            "Domain rule violation. TraceId: {TraceId}, Path: {Path}",
            context.TraceIdentifier,
            context.Request.Path);

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            Code: "DOMAIN_RULE_VIOLATION",
            Message: exception.Message,
            TraceId: context.TraceIdentifier);

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            response,
            JsonOptions,
            context.RequestAborted);
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            Code: "INTERNAL_SERVER_ERROR",
            Message: "An unexpected error occurred.",
            TraceId: context.TraceIdentifier);

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            response,
            JsonOptions,
            context.RequestAborted);
    }
}
