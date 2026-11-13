
using LIMS.Api.Configuration;
using LIMS.Api.Middleware;
using LIMS.Api.Security.Development;
using LIMS.Application;
using LIMS.Infrastructure;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLimsLogging();

builder.Services
    .AddControllers()
    .AddLimsJsonOptions();

builder.Services.AddApiBehaviorConfiguration();
builder.Services.AddApiResponseConfiguration();

builder.Services.AddLimsApiVersioning();
builder.Services.AddLimsRequestSizeLimits();
builder.Services.AddLimsRateLimiting();
builder.Services.AddLimsCors(builder.Configuration);
builder.Services.AddLimsAuthorization();

// TEMPORARY: Development-only authentication.
// Remove after Development API tests.
if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddAuthentication(
            DevelopmentAuthenticationHandler.SchemeName)
        .AddScheme<
            AuthenticationSchemeOptions,
            DevelopmentAuthenticationHandler>(
            DevelopmentAuthenticationHandler.SchemeName,
            _ => { });
}

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "LIMS API",
            Version = "v1",
            Description = "Laboratory Information Management System API"
        });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDevelopmentInventoryInMemory();
}

builder.Services.AddLimsHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "LIMS API v1");

        options.RoutePrefix = "swagger";
        options.DocumentTitle = "LIMS API Documentation";
    });

    // TEMPORARY: Remove after Development API tests.
    app.UseAuthentication();
}

app.UseHttpsRedirection();
app.UseCors(CorsConfiguration.PolicyName);
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers()
    .RequireRateLimiting(RateLimitingConfiguration.DefaultPolicy);

app.MapLimsHealthChecks();

app.Run();

public partial class Program;
