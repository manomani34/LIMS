using LIMS.Domain.Samples;
using LIMS.Domain.Samples.Enums;
using LIMS.Domain.Samples.Events;
using LIMS.Domain.Samples.Exceptions;

namespace LIMS.UnitTests.Domain.Samples;

public sealed class SampleTests
{
    private static readonly Guid LaboratoryId =
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly DateTimeOffset Now =
        new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Register_ShouldCreateSampleWithRegisteredStatus()
    {
        var sample = CreateSample();

        Assert.NotEqual(Guid.Empty, sample.Id);
        Assert.Equal(LaboratoryId, sample.LaboratoryId);
        Assert.Equal("SMP-001", sample.SampleCode);
        Assert.Equal("Saffron Sample", sample.Name);
        Assert.Equal(SampleType.Powder, sample.Type);
        Assert.Equal(SamplePriority.Normal, sample.Priority);
        Assert.Equal(SampleStatus.Registered, sample.Status);
        Assert.Equal(Now, sample.RegisteredAt);

        Assert.Contains(
            sample.DomainEvents,
            domainEvent => domainEvent is SampleRegisteredDomainEvent);
    }

    [Fact]
    public void Register_ShouldNormalizeInput()
    {
        var sample = Sample.Register(
            LaboratoryId,
            " SMP-001 ",
            " Saffron Sample ",
            SampleType.Powder,
            SamplePriority.High,
            " Description ",
            " Farm A ",
            " user-001 ",
            Now);

        Assert.Equal("SMP-001", sample.SampleCode);
        Assert.Equal("Saffron Sample", sample.Name);
        Assert.Equal("Description", sample.Description);
        Assert.Equal("Farm A", sample.Source);
        Assert.Equal("user-001", sample.RegisteredByUserId);
    }

    [Fact]
    public void Register_ShouldRejectEmptyLaboratoryId()
    {
        Assert.Throws<SampleDomainException>(() =>
            Sample.Register(
                Guid.Empty,
                "SMP-001",
                "Sample",
                SampleType.Powder,
                SamplePriority.Normal,
                null,
                null,
                null,
                Now));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Register_ShouldRejectEmptySampleCode(string code)
    {
        Assert.Throws<SampleDomainException>(() =>
            Sample.Register(
                LaboratoryId,
                code,
                "Sample",
                SampleType.Powder,
                SamplePriority.Normal,
                null,
                null,
                null,
                Now));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Register_ShouldRejectEmptyName(string name)
    {
        Assert.Throws<SampleDomainException>(() =>
            Sample.Register(
                LaboratoryId,
                "SMP-001",
                name,
                SampleType.Powder,
                SamplePriority.Normal,
                null,
                null,
                null,
                Now));
    }

    [Fact]
    public void Receive_ShouldChangeStatus()
    {
        var sample = CreateSample();

        sample.Receive(Now.AddMinutes(10));

        Assert.Equal(SampleStatus.Received, sample.Status);
        Assert.Equal(Now.AddMinutes(10), sample.ReceivedAt);
    }

    [Fact]
    public void Receive_ShouldRejectSampleNotInRegisteredStatus()
    {
        var sample = CreateSample();

        sample.Receive(Now);

        Assert.Throws<SampleDomainException>(() =>
            sample.Receive(Now.AddMinutes(10)));
    }

    [Fact]
    public void Workflow_ShouldAllowValidStatusTransitions()
    {
        var sample = CreateSample();

        sample.Receive(Now.AddMinutes(1));
        sample.StartPreparation(Now.AddMinutes(2));
        sample.MarkReadyForAnalysis(Now.AddMinutes(3));
        sample.StartAnalysis(Now.AddMinutes(4));
        sample.CompleteAnalysis(Now.AddMinutes(5));

        Assert.Equal(
            SampleStatus.AnalysisCompleted,
            sample.Status);

        Assert.Equal(Now.AddMinutes(5), sample.UpdatedAt);
    }

    [Fact]
    public void StartPreparation_ShouldRejectInvalidPreviousStatus()
    {
        var sample = CreateSample();

        Assert.Throws<SampleDomainException>(() =>
            sample.StartPreparation(Now));
    }

    [Fact]
    public void StartAnalysis_ShouldRejectSampleNotReady()
    {
        var sample = CreateSample();

        sample.Receive(Now);
        sample.StartPreparation(Now.AddMinutes(1));

        Assert.Throws<SampleDomainException>(() =>
            sample.StartAnalysis(Now.AddMinutes(2)));
    }

    [Fact]
    public void CompleteAnalysis_ShouldRejectSampleNotUnderAnalysis()
    {
        var sample = CreateSample();

        Assert.Throws<SampleDomainException>(() =>
            sample.CompleteAnalysis(Now));
    }

    [Fact]
    public void Reject_ShouldChangeStatus()
    {
        var sample = CreateSample();

        sample.Reject(Now.AddMinutes(10));

        Assert.Equal(SampleStatus.Rejected, sample.Status);
    }

    [Fact]
    public void Reject_ShouldNotRejectCompletedSample()
    {
        var sample = CreateSample();

        CompleteWorkflow(sample);

        Assert.Throws<SampleDomainException>(() =>
            sample.Reject(Now.AddMinutes(10)));
    }

    [Fact]
    public void AssignTo_ShouldAssignUserAndRaiseEvent()
    {
        var sample = CreateSample();

        sample.AssignTo("user-001", Now.AddMinutes(10));

        Assert.Equal("user-001", sample.AssignedToUserId);

        Assert.Contains(
            sample.DomainEvents,
            domainEvent => domainEvent is SampleAssignedDomainEvent);
    }

    [Fact]
    public void AssignTo_ShouldNotRaiseDuplicateAssignmentEvent()
    {
        var sample = CreateSample();

        sample.AssignTo("user-001", Now);

        var eventCount = sample.DomainEvents.Count;

        sample.AssignTo("USER-001", Now.AddMinutes(10));

        Assert.Equal(eventCount, sample.DomainEvents.Count);
    }

    [Fact]
    public void AssignTo_ShouldRejectEmptyUserId()
    {
        var sample = CreateSample();

        Assert.Throws<SampleDomainException>(() =>
            sample.AssignTo(" ", Now));
    }

    [Fact]
    public void AssignTo_ShouldRejectCompletedSample()
    {
        var sample = CreateSample();

        CompleteWorkflow(sample);

        Assert.Throws<SampleDomainException>(() =>
            sample.AssignTo("user-001", Now.AddMinutes(10)));
    }

    [Fact]
    public void Unassign_ShouldRemoveAssignedUser()
    {
        var sample = CreateSample();

        sample.AssignTo("user-001", Now);
        sample.Unassign(Now.AddMinutes(10));

        Assert.Null(sample.AssignedToUserId);
    }

    [Fact]
    public void Archive_ShouldArchiveCompletedSample()
    {
        var sample = CreateSample();

        CompleteWorkflow(sample);

        sample.Archive(Now.AddHours(1));

        Assert.Equal(SampleStatus.Archived, sample.Status);
        Assert.Equal(Now.AddHours(1), sample.ArchivedAt);
    }

    [Fact]
    public void Archive_ShouldArchiveRejectedSample()
    {
        var sample = CreateSample();

        sample.Reject(Now);
        sample.Archive(Now.AddMinutes(10));

        Assert.Equal(SampleStatus.Archived, sample.Status);
    }

    [Fact]
    public void Archive_ShouldRejectSampleWithIncompleteWorkflow()
    {
        var sample = CreateSample();

        Assert.Throws<SampleDomainException>(() =>
            sample.Archive(Now.AddHours(1)));
    }

    [Fact]
    public void ArchivedSample_ShouldNotAllowInformationUpdate()
    {
        var sample = CreateSample();

        CompleteWorkflow(sample);
        sample.Archive(Now.AddHours(1));

        Assert.Throws<SampleDomainException>(() =>
            sample.UpdateInformation(
                "Updated Sample",
                SampleType.Liquid,
                SamplePriority.High,
                null,
                null,
                Now.AddHours(2)));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateSampleFields()
    {
        var sample = CreateSample();

        sample.UpdateInformation(
            "Updated Sample",
            SampleType.Liquid,
            SamplePriority.Urgent,
            "Updated description",
            "Updated source",
            Now.AddMinutes(10));

        Assert.Equal("Updated Sample", sample.Name);
        Assert.Equal(SampleType.Liquid, sample.Type);
        Assert.Equal(SamplePriority.Urgent, sample.Priority);
        Assert.Equal("Updated description", sample.Description);
        Assert.Equal("Updated source", sample.Source);
        Assert.Equal(Now.AddMinutes(10), sample.UpdatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldRejectChangesDuringAnalysis()
    {
        var sample = CreateSample();

        sample.Receive(Now);
        sample.StartPreparation(Now.AddMinutes(1));
        sample.MarkReadyForAnalysis(Now.AddMinutes(2));
        sample.StartAnalysis(Now.AddMinutes(3));

        Assert.Throws<SampleDomainException>(() =>
            sample.UpdateInformation(
                "Updated Sample",
                SampleType.Liquid,
                SamplePriority.High,
                null,
                null,
                Now.AddMinutes(4)));
    }

    [Fact]
    public void UpdateInformation_ShouldRejectChangesAfterAnalysis()
    {
        var sample = CreateSample();

        CompleteWorkflow(sample);

        Assert.Throws<SampleDomainException>(() =>
            sample.UpdateInformation(
                "Updated Sample",
                SampleType.Liquid,
                SamplePriority.High,
                null,
                null,
                Now.AddMinutes(10)));
    }

    [Fact]
    public void DomainEvents_ShouldContainStatusChanges()
    {
        var sample = CreateSample();

        sample.Receive(Now.AddMinutes(1));
        sample.StartPreparation(Now.AddMinutes(2));

        var statusEvents = sample.DomainEvents
            .OfType<SampleStatusChangedDomainEvent>()
            .ToArray();

        Assert.Equal(2, statusEvents.Length);
        Assert.Equal(
            SampleStatus.Registered,
            statusEvents[0].PreviousStatus);
        Assert.Equal(
            SampleStatus.Received,
            statusEvents[0].NewStatus);
        Assert.Equal(
            SampleStatus.Received,
            statusEvents[1].PreviousStatus);
        Assert.Equal(
            SampleStatus.InPreparation,
            statusEvents[1].NewStatus);
    }

    private static Sample CreateSample()
    {
        return Sample.Register(
            LaboratoryId,
            "SMP-001",
            "Saffron Sample",
            SampleType.Powder,
            SamplePriority.Normal,
            "Test sample",
            "Farm A",
            "user-001",
            Now);
    }

    private static void CompleteWorkflow(Sample sample)
    {
        sample.Receive(Now.AddMinutes(1));
        sample.StartPreparation(Now.AddMinutes(2));
        sample.MarkReadyForAnalysis(Now.AddMinutes(3));
        sample.StartAnalysis(Now.AddMinutes(4));
        sample.CompleteAnalysis(Now.AddMinutes(5));
    }
}
