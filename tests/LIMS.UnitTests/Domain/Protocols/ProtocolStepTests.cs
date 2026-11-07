using LIMS.Domain.Protocols;
using LIMS.Domain.Protocols.Enums;
using Xunit;

namespace LIMS.UnitTests.Domain.Protocols;

public sealed class ProtocolStepTests
{
    private static Protocol CreateProtocol()
    {
        return Protocol.Create(
            Guid.NewGuid(),
            "NMR-001",
            "NMR Protocol",
            null,
            "1.0",
            "user-001",
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void AddStep_ShouldAcceptValidMeasurementRange()
    {
        var protocol = CreateProtocol();

        protocol.AddStep(
            1,
            "Temperature measurement",
            "Measure sample temperature.",
            ProtocolStepType.Measurement,
            true,
            25m,
            "°C",
            20m,
            30m,
            null,
            DateTimeOffset.UtcNow);

        var step = Assert.Single(protocol.Steps);

        Assert.Equal(25m, step.TargetValue);
        Assert.Equal(20m, step.MinimumValue);
        Assert.Equal(30m, step.MaximumValue);
    }

    [Fact]
    public void AddStep_ShouldRejectMinimumGreaterThanMaximum()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                1,
                "Temperature",
                "Measure temperature.",
                ProtocolStepType.Measurement,
                true,
                null,
                "°C",
                40m,
                20m,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldRejectTargetBelowMinimum()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                1,
                "Temperature",
                "Measure temperature.",
                ProtocolStepType.Measurement,
                true,
                15m,
                "°C",
                20m,
                30m,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldRejectTargetAboveMaximum()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                1,
                "Temperature",
                "Measure temperature.",
                ProtocolStepType.Measurement,
                true,
                35m,
                "°C",
                20m,
                30m,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldRejectInvalidSequence()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                0,
                "Temperature",
                "Measure temperature.",
                ProtocolStepType.Measurement,
                true,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldRejectEmptyTitle()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                1,
                " ",
                "Measure temperature.",
                ProtocolStepType.Measurement,
                true,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddStep_ShouldRejectInvalidStepType()
    {
        var protocol = CreateProtocol();

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.AddStep(
                1,
                "Temperature",
                "Measure temperature.",
                (ProtocolStepType)999,
                true,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow));
    }
}
