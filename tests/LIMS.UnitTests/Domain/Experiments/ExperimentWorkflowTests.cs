using LIMS.Domain.Experiments;
using LIMS.Domain.Experiments.Enums;

namespace LIMS.UnitTests.Domain.Experiments;

public sealed class ExperimentWorkflowTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 11, 6, 10, 0, 0, TimeSpan.Zero);

    private static Experiment CreateExperiment()
    {
        return Experiment.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "EXP-001",
            "NMR Analysis",
            null,
            "user-001",
            Now);
    }

    [Fact]
    public void Workflow_ShouldCompleteSuccessfully()
    {
        var experiment = CreateExperiment();

        experiment.AssignTo("researcher-001", Now.AddMinutes(5));
        experiment.Start(Now.AddMinutes(10));
        experiment.Complete(Now.AddMinutes(60));
        experiment.Archive(Now.AddMinutes(70));

        Assert.Equal(ExperimentStatus.Archived, experiment.Status);
        Assert.Equal("researcher-001", experiment.AssignedToUserId);
        Assert.Equal(Now.AddMinutes(10), experiment.StartedAt);
        Assert.Equal(Now.AddMinutes(60), experiment.CompletedAt);
        Assert.Equal(Now.AddMinutes(70), experiment.ArchivedAt);
    }

    [Fact]
    public void Start_ShouldRejectExperimentThatIsNotPlanned()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddMinutes(10));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Start(Now.AddMinutes(20)));
    }

    [Fact]
    public void Complete_ShouldRejectExperimentThatIsNotInProgress()
    {
        var experiment = CreateExperiment();

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Complete(Now.AddMinutes(10)));
    }

    [Fact]
    public void Cancel_ShouldRejectCompletedExperiment()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddMinutes(10));
        experiment.Complete(Now.AddMinutes(20));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Cancel("Cancellation reason", Now.AddMinutes(30)));
    }

    [Fact]
    public void Cancel_ShouldRejectAlreadyCancelledExperiment()
    {
        var experiment = CreateExperiment();

        experiment.Cancel("Initial cancellation", Now.AddMinutes(10));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Cancel("Another cancellation", Now.AddMinutes(20)));
    }

    [Fact]
    public void Archive_ShouldRejectPlannedExperiment()
    {
        var experiment = CreateExperiment();

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Archive(Now.AddMinutes(10)));
    }

    [Fact]
    public void Archive_ShouldRejectInProgressExperiment()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddMinutes(10));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Archive(Now.AddMinutes(20)));
    }

    [Fact]
    public void UpdateInformation_ShouldRejectInProgressExperiment()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddMinutes(10));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.UpdateInformation(
                "Updated title",
                null,
                Now.AddMinutes(20)));
    }

    [Fact]
    public void UpdateInformation_ShouldRejectCompletedExperiment()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddMinutes(10));
        experiment.Complete(Now.AddMinutes(20));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.UpdateInformation(
                "Updated title",
                null,
                Now.AddMinutes(30)));
    }

    [Fact]
    public void AssignTo_ShouldRejectTerminalExperiment()
    {
        var experiment = CreateExperiment();

        experiment.Cancel("Cancelled", Now.AddMinutes(10));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.AssignTo("researcher-001", Now.AddMinutes(20)));
    }

    [Fact]
    public void Unassign_ShouldRejectTerminalExperiment()
    {
        var experiment = CreateExperiment();

        experiment.AssignTo("researcher-001", Now.AddMinutes(10));
        experiment.Cancel("Cancelled", Now.AddMinutes(20));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Unassign(Now.AddMinutes(30)));
    }

    [Fact]
    public void ArchivedExperiment_ShouldRejectFurtherCancellation()
    {
        var experiment = CreateExperiment();

        experiment.Cancel("Cancelled", Now.AddMinutes(10));
        experiment.Archive(Now.AddMinutes(20));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Cancel("Another reason", Now.AddMinutes(30)));
    }
}
