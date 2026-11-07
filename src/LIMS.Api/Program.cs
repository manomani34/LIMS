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
builder.Services.AddLimsAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddLimsHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
