
using System.Security.Claims;
using LIMS.Api.Authorization.Permissions;
using LIMS.Application.Abstractions.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LIMS.IntegrationTests.Authorization;

public sealed class PermissionAuthorizationTests
{
    private const string Permission =
        "Laboratory.Experiments.Create";

    [Fact]
    public async Task Handler_ShouldSucceed_WhenUserHasPermission()
    {
        var checker = new FakePermissionChecker(true);
        var handler = new PermissionAuthorizationHandler(checker);

        var requirement = new PermissionRequirement(Permission);

        var context = CreateAuthorizationContext(requirement);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
        Assert.Equal(Permission, checker.LastCheckedPermission);
    }

    [Fact]
    public async Task Handler_ShouldNotSucceed_WhenUserDoesNotHavePermission()
    {
        var checker = new FakePermissionChecker(false);
        var handler = new PermissionAuthorizationHandler(checker);

        var requirement = new PermissionRequirement(Permission);

        var context = CreateAuthorizationContext(requirement);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task PolicyProvider_ShouldCreatePermissionPolicy()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync(
            $"Permission:{Permission}");

        Assert.NotNull(policy);

        Assert.Contains(
            JwtBearerDefaults.AuthenticationScheme,
            policy.AuthenticationSchemes);

        var requirement = Assert.Single(
            policy.Requirements.OfType<PermissionRequirement>());

        Assert.Equal(Permission, requirement.Permission);
        Assert.Contains(
            policy.Requirements,
            requirement => requirement is DenyAnonymousAuthorizationRequirement);
    }

    [Fact]
    public async Task PolicyProvider_ShouldReturnNull_WhenPermissionIsEmpty()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync("Permission:");

        Assert.Null(policy);
    }

    [Fact]
    public async Task PolicyProvider_ShouldDelegateUnknownPolicy()
    {
        var provider = CreatePolicyProvider();

        var policy = await provider.GetPolicyAsync("SomeOtherPolicy");

        Assert.Null(policy);
    }

    private static AuthorizationHandlerContext CreateAuthorizationContext(
        IAuthorizationRequirement requirement)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim("sub", "test-user"),
                new Claim("name", "Test User")
            ],
            authenticationType: JwtBearerDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        return new AuthorizationHandlerContext(
            [requirement],
            principal,
            resource: null);
    }

    private static PermissionPolicyProvider CreatePolicyProvider()
    {
        var options = Options.Create(new AuthorizationOptions());

        return new PermissionPolicyProvider(options);
    }

    private sealed class FakePermissionChecker : IPermissionChecker
    {
        private readonly bool _hasPermission;

        public string? LastCheckedPermission { get; private set; }

        public FakePermissionChecker(bool hasPermission)
        {
            _hasPermission = hasPermission;
        }

        public Task<bool> HasPermissionAsync(
            string permission,
            CancellationToken cancellationToken = default)
        {
            LastCheckedPermission = permission;

            return Task.FromResult(_hasPermission);
        }
    }
}
