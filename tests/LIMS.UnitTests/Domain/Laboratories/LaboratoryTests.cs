using LIMS.Domain.Laboratories;
using LIMS.Domain.Laboratories.Enums;
using LIMS.Domain.Laboratories.Events;
using LIMS.Domain.Laboratories.Exceptions;

namespace LIMS.UnitTests.Domain.Laboratories;

public sealed class LaboratoryTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_ShouldCreateActiveLaboratory()
    {
        var laboratory = Laboratory.Create(
            " Analytical Chemistry ",
            " ac-001 ",
            " Main laboratory ",
            Now);

        Assert.NotEqual(Guid.Empty, laboratory.Id);
        Assert.Equal("Analytical Chemistry", laboratory.Name);
        Assert.Equal("AC-001", laboratory.Code);
        Assert.Equal("Main laboratory", laboratory.Description);
        Assert.Equal(LaboratoryStatus.Active, laboratory.Status);
        Assert.Equal(Now, laboratory.CreatedAt);

        var domainEvent = Assert.Single(laboratory.DomainEvents);
        Assert.IsType<LaboratoryCreatedDomainEvent>(domainEvent);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldRejectEmptyName(string name)
    {
        Assert.Throws<LaboratoryDomainException>(() =>
            Laboratory.Create(name, "LAB-001", null, Now));
    }

    [Fact]
    public void Create_ShouldRejectNameLongerThan150Characters()
    {
        var name = new string('A', 151);

        Assert.Throws<LaboratoryDomainException>(() =>
            Laboratory.Create(name, "LAB-001", null, Now));
    }

    [Fact]
    public void Create_ShouldNormalizeLaboratoryCode()
    {
        var laboratory = Laboratory.Create(
            "Laboratory",
            " lab_001 ",
            null,
            Now);

        Assert.Equal("LAB_001", laboratory.Code);
    }

    [Theory]
    [InlineData("LAB 001")]
    [InlineData("LAB@001")]
    [InlineData("LAB.001")]
    public void Create_ShouldRejectInvalidCode(string code)
    {
        Assert.Throws<LaboratoryDomainException>(() =>
            Laboratory.Create("Laboratory", code, null, Now));
    }

    [Fact]
    public void AddMember_ShouldAddMemberAndRaiseDomainEvent()
    {
        var laboratory = CreateLaboratory();

        laboratory.AddMember(
            "user-001",
            LaboratoryMemberRole.Researcher,
            Now);

        var member = Assert.Single(laboratory.Members);

        Assert.Equal("user-001", member.UserId);
        Assert.Equal(LaboratoryMemberRole.Researcher, member.Role);
        Assert.Equal(Now, member.JoinedAt);

        Assert.Contains(
            laboratory.DomainEvents,
            domainEvent =>
                domainEvent is LaboratoryMemberAddedDomainEvent);
    }

    [Fact]
    public void AddMember_ShouldRejectDuplicateUser()
    {
        var laboratory = CreateLaboratory();

        laboratory.AddMember(
            "user-001",
            LaboratoryMemberRole.Researcher,
            Now);

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.AddMember(
                "USER-001",
                LaboratoryMemberRole.Technician,
                Now));
    }

    [Fact]
    public void AddMember_ShouldRejectEmptyUserId()
    {
        var laboratory = CreateLaboratory();

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.AddMember(
                " ",
                LaboratoryMemberRole.Researcher,
                Now));
    }

    [Fact]
    public void ChangeMemberRole_ShouldUpdateMemberRole()
    {
        var laboratory = CreateLaboratory();

        laboratory.AddMember(
            "user-001",
            LaboratoryMemberRole.Researcher,
            Now);

        laboratory.ChangeMemberRole(
            "user-001",
            LaboratoryMemberRole.LaboratoryManager,
            Now.AddMinutes(5));

        var member = Assert.Single(laboratory.Members);

        Assert.Equal(
            LaboratoryMemberRole.LaboratoryManager,
            member.Role);
    }

    [Fact]
    public void ChangeMemberRole_ShouldRejectUnknownMember()
    {
        var laboratory = CreateLaboratory();

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.ChangeMemberRole(
                "unknown-user",
                LaboratoryMemberRole.Researcher,
                Now));
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMemberAndRaiseDomainEvent()
    {
        var laboratory = CreateLaboratory();

        laboratory.AddMember(
            "user-001",
            LaboratoryMemberRole.Researcher,
            Now);

        laboratory.RemoveMember("user-001", Now.AddMinutes(10));

        Assert.Empty(laboratory.Members);

        Assert.Contains(
            laboratory.DomainEvents,
            domainEvent =>
                domainEvent is LaboratoryMemberRemovedDomainEvent);
    }

    [Fact]
    public void RemoveMember_ShouldRejectUnknownMember()
    {
        var laboratory = CreateLaboratory();

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.RemoveMember("unknown-user", Now));
    }

    [Fact]
    public void ChangeStatus_ShouldChangeStatusAndRaiseDomainEvent()
    {
        var laboratory = CreateLaboratory();

        laboratory.ChangeStatus(
            LaboratoryStatus.Inactive,
            Now.AddMinutes(10));

        Assert.Equal(
            LaboratoryStatus.Inactive,
            laboratory.Status);

        Assert.Contains(
            laboratory.DomainEvents,
            domainEvent =>
                domainEvent is LaboratoryStatusChangedDomainEvent);
    }

    [Fact]
    public void ChangeStatus_ShouldNotRaiseEventWhenStatusIsUnchanged()
    {
        var laboratory = CreateLaboratory();
        var initialEventCount = laboratory.DomainEvents.Count;

        laboratory.ChangeStatus(
            LaboratoryStatus.Active,
            Now.AddMinutes(10));

        Assert.Equal(initialEventCount, laboratory.DomainEvents.Count);
    }

    [Fact]
    public void InactiveLaboratory_ShouldNotAcceptNewMembers()
    {
        var laboratory = CreateLaboratory();

        laboratory.ChangeStatus(
            LaboratoryStatus.Inactive,
            Now);

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.AddMember(
                "user-001",
                LaboratoryMemberRole.Researcher,
                Now));
    }

    [Fact]
    public void ArchivedLaboratory_ShouldNotAllowInformationUpdate()
    {
        var laboratory = CreateLaboratory();

        laboratory.ChangeStatus(
            LaboratoryStatus.Archived,
            Now);

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.UpdateInformation(
                "Updated Laboratory",
                "LAB-002",
                null,
                Now.AddMinutes(10)));
    }

    [Fact]
    public void ArchivedLaboratory_ShouldNotAllowMemberRemoval()
    {
        var laboratory = CreateLaboratory();

        laboratory.AddMember(
            "user-001",
            LaboratoryMemberRole.Researcher,
            Now);

        laboratory.ChangeStatus(
            LaboratoryStatus.Archived,
            Now.AddMinutes(5));

        Assert.Throws<LaboratoryDomainException>(() =>
            laboratory.RemoveMember(
                "user-001",
                Now.AddMinutes(10)));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateLaboratoryFields()
    {
        var laboratory = CreateLaboratory();
        var updatedAt = Now.AddHours(1);

        laboratory.UpdateInformation(
            "Updated Laboratory",
            "LAB-002",
            "Updated description",
            updatedAt);

        Assert.Equal("Updated Laboratory", laboratory.Name);
        Assert.Equal("LAB-002", laboratory.Code);
        Assert.Equal("Updated description", laboratory.Description);
        Assert.Equal(updatedAt, laboratory.UpdatedAt);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var laboratory = CreateLaboratory();

        Assert.NotEmpty(laboratory.DomainEvents);

        laboratory.ClearDomainEvents();

        Assert.Empty(laboratory.DomainEvents);
    }

    private static Laboratory CreateLaboratory()
    {
        return Laboratory.Create(
            "Analytical Chemistry",
            "AC-001",
            null,
            Now);
    }
}
