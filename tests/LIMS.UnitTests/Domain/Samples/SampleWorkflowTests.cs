using LIMS.Domain.Samples;
using LIMS.Domain.Samples.Enums;
using LIMS.Domain.Samples.Exceptions;

namespace LIMS.UnitTests.Domain.Samples;

public sealed class SampleWorkflowTests
{
    private static readonly Guid LaboratoryId = Guid.NewGuid();

    private static readonly DateTimeOffset Now =
        new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CompleteWorkflow_ShouldFollowExpectedSequence()
    {
        var sample = CreateSample();

        Assert.Equal(SampleStatus.Registered, sample.Status);

        sample.Receive(Now.AddMinutes(1));
        Assert.Equal(SampleStatus.Received, sample.Status);

        sample.StartPreparation(Now.AddMinutes(2));
        Assert.Equal(SampleStatus.InPreparation, sample.Status);

        sample.MarkReadyForAnalysis(Now.AddMinutes(3));
        Assert.Equal(SampleStatus.ReadyForAnalysis, sample.Status);

        sample.StartAnalysis(Now.AddMinutes(4));
        Assert.Equal(SampleStatus.UnderAnalysis, sample.Status);

        sample.CompleteAnalysis(Now.AddMinutes(5));
        Assert.Equal(SampleStatus.AnalysisCompleted, sample.Status);
    }

    [Fact]
    public void RejectedSample_ShouldNotBeAssigned()
    {
        var sample = CreateSample();

        sample.Reject(Now);

        Assert.Throws<SampleDomainException>(() =>
            sample.AssignTo("user-001", Now.AddMinutes(1)));
    }

    [Fact]
    public void ArchivedSample_ShouldNotBeAssigned()
    {
        var sample = CreateSample();

        sample.Reject(Now);
        sample.Archive(Now.AddMinutes(1));

        Assert.Throws<SampleDomainException>(() =>
            sample.AssignTo("user-001", Now.AddMinutes(2)));
    }

    [Fact]
    public void ArchivedSample_ShouldNotBeReceived()
    {
        var sample = CreateSample();

        sample.Reject(Now);
        sample.Archive(Now.AddMinutes(1));

        Assert.Throws<SampleDomainException>(() =>
            sample.Receive(Now.AddMinutes(2)));
    }

    [Fact]
    public void RejectedSample_ShouldNotBeArchivedTwiceWithDifferentTimestamp()
    {
        var sample = CreateSample();

        sample.Reject(Now);
        sample.Archive(Now.AddMinutes(1));

        var archivedAt = sample.ArchivedAt;

        sample.Archive(Now.AddMinutes(2));

        Assert.Equal(archivedAt, sample.ArchivedAt);
    }

    private static Sample CreateSample()
    {
        return Sample.Register(
            LaboratoryId,
            "SMP-001",
            "Test Sample",
            SampleType.Solid,
            SamplePriority.Normal,
            null,
            null,
            "user-001",
            Now);
    }
}
