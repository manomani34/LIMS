using LIMS.Domain.Protocols;
using LIMS.Domain.Protocols.Enums;
using Xunit;

namespace LIMS.UnitTests.Domain.Protocols;

public sealed class ProtocolTests
{
    private static Protocol CreateProtocol()
    {
        return Protocol.Create(
            Guid.NewGuid(),
            "NMR-PROTOCOL-001",
            "NMR Analysis",
            "Standard NMR analysis protocol.",
            "1.0",
            "user-001",
            DateTimeOffset.UtcNow);
    }

    private static void AddValidStep(Protocol protocol)
    {
        protocol.AddStep(
            1,
            "Sample preparation",
            "Prepare the sample for NMR analysis.",
            ProtocolStepType.Preparation,
            true,
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_ShouldInitializeDraftProtocol()
    {
        var laboratoryId = Guid.NewGuid();

        var protocol = Protocol.Create(
            laboratoryId,
            "NMR-001",
            "NMR Protocol",
            null,
            "1.0",
            "user-001",
            DateTimeOffset.UtcNow);

        Assert.NotEqual(Guid.Empty, protocol.Id);
        Assert.Equal(laboratoryId, protocol.LaboratoryId);
        Assert.Equal("NMR-001", protocol.ProtocolCode);
        Assert.Equal("NMR Protocol", protocol.Name);
        Assert.Equal(ProtocolStatus.Draft, protocol.Status);
        Assert.Empty(protocol.Steps);
        Assert.Single(protocol.DomainEvents);
    }

    [Fact]
    public void Create_ShouldNormalizeProtocolCode()
    {
        var protocol = Protocol.Create(
            Guid.NewGuid(),
            " nmr-001 ",
            "NMR Protocol",
            null,
            "1.0",
            "user-001",
            DateTimeOffset.UtcNow);

        Assert.Equal("NMR-001", protocol.ProtocolCode);
    }

    [Fact]
    public void Create_ShouldRejectEmptyLaboratoryId()
    {
        Assert.Throws<ProtocolDomainException>(() =>
            Protocol.Create(
                Guid.Empty,
                "NMR-001",
                "NMR Protocol",
                null,
                "1.0",
                "user-001",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Create_ShouldRejectInvalidProtocolCode()
    {
        Assert.Throws<ProtocolDomainException>(() =>
            Protocol.Create(
                Guid.NewGuid(),
                "NMR 001!",
                "NMR Protocol",
                null,
                "1.0",
                "user-001",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateDraftProtocol()
    {
        var protocol = CreateProtocol();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(5);

        protocol.UpdateInformation(
            "Updated NMR Protocol",
            "Updated description.",
            "2.0",
            updatedAt);

        Assert.Equal("Updated NMR Protocol", protocol.Name);
        Assert.Equal("Updated description.", protocol.Description);
        Assert.Equal("2.0", protocol.Version);
        Assert.Equal(updatedAt, protocol.UpdatedAt);
    }

    [Fact]
    public void AddStep_ShouldAddStepToDraftProtocol()
    {
        var protocol = CreateProtocol();

        AddValidStep(protocol);

        var step = Assert.Single(protocol.Steps);

        Assert.Equal(1, step.Sequence);
        Assert.Equal("Sample preparation", step.Title);
        Assert.Equal(ProtocolStepType.Preparation, step.Type);
        Assert.True(step.IsRequired);
    }

    [Fact]
    public void AddStep_ShouldRejectDuplicateSequence()
    {
        var protocol = CreateProtocol();

        AddValidStep(protocol);

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                1,
                "Second step",
                "Another instruction.",
                ProtocolStepType.Instruction,
                true,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldSortStepsBySequence()
    {
        var protocol = CreateProtocol();

        protocol.AddStep(
            3,
            "Third step",
            "Third instruction.",
            ProtocolStepType.Instruction,
            true,
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        protocol.AddStep(
            1,
            "First step",
            "First instruction.",
            ProtocolStepType.Instruction,
            true,
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        protocol.AddStep(
            2,
            "Second step",
            "Second instruction.",
            ProtocolStepType.Instruction,
            true,
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Assert.Equal(new[] { 1, 2, 3 }, protocol.Steps.Select(x => x.Sequence));
    }

    [Fact]
    public void UpdateStep_ShouldUpdateExistingStep()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);

        var step = protocol.Steps.Single();

        protocol.UpdateStep(
            step.Id,
            2,
            "Updated preparation",
            "Updated instruction.",
            ProtocolStepType.Instruction,
            false,
            25m,
            "°C",
            20m,
            30m,
            "Keep temperature controlled.",
            DateTimeOffset.UtcNow.AddMinutes(2));

        var updatedStep = protocol.Steps.Single();

        Assert.Equal(2, updatedStep.Sequence);
        Assert.Equal("Updated preparation", updatedStep.Title);
        Assert.Equal(ProtocolStepType.Instruction, updatedStep.Type);
        Assert.False(updatedStep.IsRequired);
        Assert.Equal(25m, updatedStep.TargetValue);
        Assert.Equal("°C", updatedStep.TargetUnit);
    }

    [Fact]
    public void UpdateStep_ShouldRejectUnknownStep()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.UpdateStep(
                Guid.NewGuid(),
                1,
                "Unknown",
                "Instruction.",
                ProtocolStepType.Instruction,
                true,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void RemoveStep_ShouldRemoveExistingStep()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);

        var stepId = protocol.Steps.Single().Id;

        protocol.RemoveStep(stepId, DateTimeOffset.UtcNow);

        Assert.Empty(protocol.Steps);
    }

    [Fact]
    public void Activate_ShouldRequireAtLeastOneStep()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.Activate(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Activate_ShouldActivateProtocolWithSteps()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);

        var activatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        protocol.Activate(activatedAt);

        Assert.Equal(ProtocolStatus.Active, protocol.Status);
        Assert.Equal(activatedAt, protocol.ActivatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldRejectActiveProtocol()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);
        protocol.Activate(DateTimeOffset.UtcNow);

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.UpdateInformation(
                "Changed name",
                null,
                "2.0",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldRejectActiveProtocol()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);
        protocol.Activate(DateTimeOffset.UtcNow);

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                2,
                "Another step",
                "Another instruction.",
                ProtocolStepType.Instruction,
                true,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Deprecate_ShouldChangeStatus()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);
        protocol.Activate(DateTimeOffset.UtcNow);

        var deprecatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        protocol.Deprecate(deprecatedAt);

        Assert.Equal(ProtocolStatus.Deprecated, protocol.Status);
        Assert.Equal(deprecatedAt, protocol.DeprecatedAt);
    }

    [Fact]
    public void Archive_ShouldRequireDeprecatedStatus()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);
        protocol.Activate(DateTimeOffset.UtcNow);

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.Archive(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Archive_ShouldArchiveDeprecatedProtocol()
    {
        var protocol = CreateProtocol();
        AddValidStep(protocol);
        protocol.Activate(DateTimeOffset.UtcNow);
        protocol.Deprecate(DateTimeOffset.UtcNow.AddMinutes(1));

        var archivedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        protocol.Archive(archivedAt);

        Assert.Equal(ProtocolStatus.Archived, protocol.Status);
        Assert.Equal(archivedAt, protocol.ArchivedAt);
    }

    [Fact]
    public void Deprecate_ShouldRejectDraftProtocol()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.Deprecate(DateTimeOffset.UtcNow));
    }
}
