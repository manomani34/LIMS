using LIMS.Api.Configuration;
using LIMS.Api.Middleware;
using LIMS.Application;
using LIMS.Infrastructure;

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

builder.Services.AddLimsAuthentication(
    builder.Configuration);

builder.Services.AddLimsAuthorization();

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
}

app.UseHttpsRedirection();

app.UseCors(CorsConfiguration.PolicyName);

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers()
    .RequireRateLimiting(RateLimitingConfiguration.DefaultPolicy);

app.MapLimsHealthChecks();

app.Run();

public partial class Program;
