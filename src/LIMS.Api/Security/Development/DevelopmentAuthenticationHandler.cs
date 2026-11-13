
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LIMS.Api.Security.Development;

// TEMPORARY: Remove after Development API tests.
public sealed class DevelopmentAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Development";

    private const string DevelopmentUserId = "dev-user-001";

    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                DevelopmentUserId),

            new Claim(
                ClaimTypes.Name,
                "Development Test User")
        };

        var identity = new ClaimsIdentity(
            claims,
            SchemeName);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            SchemeName);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}
