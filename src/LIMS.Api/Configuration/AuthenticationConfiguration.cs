
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LIMS.Api.Configuration;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddLimsAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Authentication");

        var issuer = section["Issuer"];
        var audience = section["Audience"];
        var publicKeyPem = section["PublicKeyPem"];

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException(
                "Authentication:Issuer is not configured.");
        }

        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException(
                "Authentication:Audience is not configured.");
        }

        if (string.IsNullOrWhiteSpace(publicKeyPem))
        {
            throw new InvalidOperationException(
                "Authentication:PublicKeyPem is not configured.");
        }

        // Validate the public key immediately.
        using (var validationRsa = RSA.Create())
        {
            validationRsa.ImportFromPem(publicKeyPem);
        }

        // Keep RSA alive for the application's lifetime.
        // The DI container disposes it when the application stops.
        services.AddSingleton<RSA>(_ =>
        {
            var rsa = RSA.Create();

            try
            {
                rsa.ImportFromPem(publicKeyPem);
                return rsa;
            }
            catch
            {
                rsa.Dispose();
                throw;
            }
        });

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = false;
                options.IncludeErrorDetails = false;

                // Preserve original JWT claim names.
                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                        ClockSkew = TimeSpan.FromSeconds(30),

                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var subject =
                            context.Principal?.FindFirst("sub")?.Value;

                        if (string.IsNullOrWhiteSpace(subject))
                        {
                            context.Fail(
                                "The token does not contain a valid subject.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        // Inject the singleton RSA key into JWT validation options.
        services
            .AddOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme)
            .Configure<RSA>((options, rsa) =>
            {
                options.TokenValidationParameters.IssuerSigningKey =
                    new RsaSecurityKey(rsa);
            });

        return services;
    }
}
