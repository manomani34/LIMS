
using System.Security.Claims;
using LIMS.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Http;

namespace LIMS.Infrastructure.Identity.Users;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal =>
        _httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        Principal?.FindFirstValue("sub")
        ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName =>
        Principal?.FindFirstValue("name")
        ?? Principal?.FindFirstValue("preferred_username")
        ?? Principal?.Identity?.Name;

    public string? Email =>
        Principal?.FindFirstValue("email")
        ?? Principal?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyCollection<string> Roles =>
        Principal?
            .FindAll("role")
            .Concat(Principal.FindAll(ClaimTypes.Role))
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? [];

    public IReadOnlyCollection<string> Permissions =>
        Principal?
            .FindAll("Permission")
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? [];
}
