using LIMS.Domain.Protocols;
using LIMS.Domain.Protocols.Enums;
using Xunit;

namespace LIMS.UnitTests.Domain.Protocols;

public sealed class ProtocolWorkflowTests
{
    [Fact]
    public void CompleteLifecycle_ShouldFollowExpectedTransitions()
    {
        var now = DateTimeOffset.UtcNow;

        var protocol = Protocol.Create(
            Guid.NewGuid(),
            "NMR-LIFECYCLE-001",
            "NMR Lifecycle Protocol",
            "Lifecycle test.",
            "1.0",
            "user-001",
            now);

        protocol.AddStep(
            1,
            "Prepare sample",
            "Prepare sample for analysis.",
            ProtocolStepType.Preparation,
            true,
            null,
            null,
            null,
            null,
            null,
            now.AddMinutes(1));

        protocol.AddStep(
            2,
            "Measure sample",
            "Acquire NMR spectrum.",
            ProtocolStepType.Measurement,
            true,
            25m,
            "°C",
            20m,
            30m,
            null,
            now.AddMinutes(2));

        Assert.Equal(ProtocolStatus.Draft, protocol.Status);
        Assert.Equal(2, protocol.Steps.Count);

        protocol.Activate(now.AddMinutes(3));

        Assert.Equal(ProtocolStatus.Active, protocol.Status);

        protocol.Deprecate(now.AddMinutes(4));

        Assert.Equal(ProtocolStatus.Deprecated, protocol.Status);

        protocol.Archive(now.AddMinutes(5));

        Assert.Equal(ProtocolStatus.Archived, protocol.Status);
        Assert.NotNull(protocol.ActivatedAt);
        Assert.NotNull(protocol.DeprecatedAt);
        Assert.NotNull(protocol.ArchivedAt);
    }

    [Fact]
    public void ArchivedProtocol_ShouldNotBeReactivated()
    {
        var protocol = Protocol.Create(
            Guid.NewGuid(),
            "NMR-ARCHIVE-001",
            "Archived Protocol",
            null,
            "1.0",
            "user-001",
            DateTimeOffset.UtcNow);

        protocol.AddStep(
            1,
            "Prepare",
            "Prepare sample.",
            ProtocolStepType.Preparation,
            true,
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        protocol.Activate(DateTimeOffset.UtcNow);
        protocol.Deprecate(DateTimeOffset.UtcNow);
        protocol.Archive(DateTimeOffset.UtcNow);

        Assert.Throws<ProtocolDomainException>(() =>
            protocol.Activate(DateTimeOffset.UtcNow));
    }
}
