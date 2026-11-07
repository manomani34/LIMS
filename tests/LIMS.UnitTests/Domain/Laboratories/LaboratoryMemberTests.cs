using LIMS.Domain.Laboratories.Entities;
using LIMS.Domain.Laboratories.Enums;
using LIMS.Domain.Laboratories.Exceptions;

namespace LIMS.UnitTests.Domain.Laboratories;

public sealed class LaboratoryMemberTests
{
    private static readonly Guid LaboratoryId =
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly DateTimeOffset JoinedAt =
        new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_ShouldCreateValidMember()
    {
        var member = CreateMember();

        Assert.NotEqual(Guid.Empty, member.Id);
        Assert.Equal(LaboratoryId, member.LaboratoryId);
        Assert.Equal("user-001", member.UserId);
        Assert.Equal(LaboratoryMemberRole.Researcher, member.Role);
        Assert.Equal(JoinedAt, member.JoinedAt);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyLaboratoryId()
    {
        Assert.Throws<LaboratoryDomainException>(() =>
            new LaboratoryMember(
                Guid.Empty,
                "user-001",
                LaboratoryMemberRole.Researcher,
                JoinedAt));
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyUserId()
    {
        Assert.Throws<LaboratoryDomainException>(() =>
            new LaboratoryMember(
                LaboratoryId,
                " ",
                LaboratoryMemberRole.Researcher,
                JoinedAt));
    }

    private static LaboratoryMember CreateMember()
    {
        return new LaboratoryMember(
            LaboratoryId,
            " user-001 ",
            LaboratoryMemberRole.Researcher,
            JoinedAt);
    }
}
