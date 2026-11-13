
using LIMS.Domain.Laboratories;
using LIMS.Domain.Laboratories.Enums;
using LIMS.Domain.Laboratories.Exceptions;
using Xunit;

namespace LIMS.UnitTests.Domain.Laboratories;

public sealed class LaboratoryMemberTests
{
    private static Laboratory CreateLaboratory()
    {
        return Laboratory.Create(
            "Chemistry Laboratory",
            "CHEM-001",
            "Research laboratory",
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void AddMember_ShouldAddMemberToLaboratory()
    {
        var laboratory = CreateLaboratory();
        var joinedAt = DateTimeOffset.UtcNow;
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        laboratory.AddMember("user-01", role, joinedAt);

        var member = Assert.Single(laboratory.Members);

        Assert.Equal(laboratory.Id, member.LaboratoryId);
        Assert.Equal("user-01", member.UserId);
        Assert.Equal(role, member.Role);
        Assert.Equal(joinedAt, member.JoinedAt);
    }

    [Fact]
    public void AddMember_ShouldNormalizeUserId()
    {
        var laboratory = CreateLaboratory();
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        laboratory.AddMember("  user-01  ", role, DateTimeOffset.UtcNow);

        Assert.Equal("user-01", Assert.Single(laboratory.Members).UserId);
    }

    [Fact]
    public void AddMember_ShouldRejectEmptyUserId()
    {
        var laboratory = CreateLaboratory();
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.AddMember(" ", role, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddMember_ShouldRejectDuplicateUserIgnoringCase()
    {
        var laboratory = CreateLaboratory();
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        laboratory.AddMember("user-01", role, DateTimeOffset.UtcNow);

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.AddMember("USER-01", role, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ChangeMemberRole_ShouldUpdateMemberRole()
    {
        var roles = Enum.GetValues<LaboratoryMemberRole>();

        Assert.True(roles.Length >= 2);

        var laboratory = CreateLaboratory();
        var originalRole = roles[0];
        var newRole = roles[1];

        laboratory.AddMember(
            "user-01",
            originalRole,
            DateTimeOffset.UtcNow);

        laboratory.ChangeMemberRole(
            "user-01",
            newRole,
            DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Equal(newRole, Assert.Single(laboratory.Members).Role);
    }

    [Fact]
    public void ChangeMemberRole_ShouldFindMemberIgnoringCase()
    {
        var roles = Enum.GetValues<LaboratoryMemberRole>();

        Assert.True(roles.Length >= 2);

        var laboratory = CreateLaboratory();

        laboratory.AddMember(
            "user-01",
            roles[0],
            DateTimeOffset.UtcNow);

        laboratory.ChangeMemberRole(
            "USER-01",
            roles[1],
            DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Equal(roles[1], Assert.Single(laboratory.Members).Role);
    }

    [Fact]
    public void ChangeMemberRole_ShouldRejectUnknownMember()
    {
        var laboratory = CreateLaboratory();
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.ChangeMemberRole(
                "unknown-user",
                role,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void RemoveMember_ShouldRemoveExistingMember()
    {
        var laboratory = CreateLaboratory();
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        laboratory.AddMember("user-01", role, DateTimeOffset.UtcNow);

        laboratory.RemoveMember(
            "user-01",
            DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Empty(laboratory.Members);
    }

    [Fact]
    public void RemoveMember_ShouldFindMemberIgnoringCase()
    {
        var laboratory = CreateLaboratory();
        var role = Enum.GetValues<LaboratoryMemberRole>().First();

        laboratory.AddMember("user-01", role, DateTimeOffset.UtcNow);

        laboratory.RemoveMember(
            "USER-01",
            DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Empty(laboratory.Members);
    }

    [Fact]
    public void RemoveMember_ShouldRejectUnknownMember()
    {
        var laboratory = CreateLaboratory();

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.RemoveMember(
                "unknown-user",
                DateTimeOffset.UtcNow));
    }
}
