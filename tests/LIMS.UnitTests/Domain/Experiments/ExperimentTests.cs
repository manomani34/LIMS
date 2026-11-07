using LIMS.Domain.Experiments;
using LIMS.Domain.Experiments.Enums;

namespace LIMS.UnitTests.Domain.Experiments;

public sealed class ExperimentTests
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
            "Initial experiment description",
            "user-001",
            Now);
    }

    [Fact]
    public void Create_ShouldInitializeExperiment()
    {
        var laboratoryId = Guid.NewGuid();
        var sampleId = Guid.NewGuid();

        var experiment = Experiment.Create(
            laboratoryId,
            sampleId,
            "exp-001",
            "NMR Analysis",
            "Description",
            "user-001",
            Now);

        Assert.NotEqual(Guid.Empty, experiment.Id);
        Assert.Equal(laboratoryId, experiment.LaboratoryId);
        Assert.Equal(sampleId, experiment.SampleId);
        Assert.Equal("EXP-001", experiment.ExperimentCode);
        Assert.Equal("NMR Analysis", experiment.Title);
        Assert.Equal("Description", experiment.Description);
        Assert.Equal(ExperimentStatus.Planned, experiment.Status);
        Assert.Equal("user-001", experiment.CreatedByUserId);
        Assert.Null(experiment.AssignedToUserId);
        Assert.Equal(Now, experiment.CreatedAt);
        Assert.Equal(Now, experiment.UpdatedAt);
        Assert.Null(experiment.StartedAt);
        Assert.Null(experiment.CompletedAt);
        Assert.Null(experiment.ArchivedAt);
    }

    [Fact]
    public void Create_ShouldNormalizeExperimentCode()
    {
        var experiment = Experiment.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  exp_001  ",
            "NMR Analysis",
            null,
            "user-001",
            Now);

        Assert.Equal("EXP_001", experiment.ExperimentCode);
    }

    [Fact]
    public void Create_ShouldRaiseCreatedDomainEvent()
    {
        var laboratoryId = Guid.NewGuid();
        var sampleId = Guid.NewGuid();

        var experiment = Experiment.Create(
            laboratoryId,
            sampleId,
            "EXP-001",
            "NMR Analysis",
            null,
            "user-001",
            Now);

        var domainEvent = Assert.Single(experiment.DomainEvents);

        Assert.Equal(
            "ExperimentCreatedDomainEvent",
            domainEvent.GetType().Name);
    }

    [Fact]
    public void Create_ShouldRejectEmptyLaboratoryId()
    {
        Assert.Throws<ExperimentDomainException>(() =>
            Experiment.Create(
                Guid.Empty,
                Guid.NewGuid(),
                "EXP-001",
                "NMR Analysis",
                null,
                "user-001",
                Now));
    }

    [Fact]
    public void Create_ShouldRejectEmptySampleId()
    {
        Assert.Throws<ExperimentDomainException>(() =>
            Experiment.Create(
                Guid.NewGuid(),
                Guid.Empty,
                "EXP-001",
                "NMR Analysis",
                null,
                "user-001",
                Now));
    }

    [Fact]
    public void Create_ShouldRejectInvalidExperimentCode()
    {
        Assert.Throws<ExperimentDomainException>(() =>
            Experiment.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "EXP 001",
                "NMR Analysis",
                null,
                "user-001",
                Now));
    }

    [Fact]
    public void Create_ShouldRejectEmptyTitle()
    {
        Assert.Throws<ExperimentDomainException>(() =>
            Experiment.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "EXP-001",
                " ",
                null,
                "user-001",
                Now));
    }

    [Fact]
    public void Create_ShouldRejectEmptyCreator()
    {
        Assert.Throws<ExperimentDomainException>(() =>
            Experiment.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "EXP-001",
                "NMR Analysis",
                null,
                " ",
                Now));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateTitleAndDescription()
    {
        var experiment = CreateExperiment();
        var updatedAt = Now.AddHours(1);

        experiment.UpdateInformation(
            "Updated NMR Analysis",
            "Updated description",
            updatedAt);

        Assert.Equal("Updated NMR Analysis", experiment.Title);
        Assert.Equal("Updated description", experiment.Description);
        Assert.Equal(updatedAt, experiment.UpdatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldRejectEmptyTitle()
    {
        var experiment = CreateExperiment();

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.UpdateInformation(" ", null, Now.AddHours(1)));
    }

    [Fact]
    public void AssignTo_ShouldAssignUser()
    {
        var experiment = CreateExperiment();
        var updatedAt = Now.AddMinutes(10);

        experiment.AssignTo("user-002", updatedAt);

        Assert.Equal("user-002", experiment.AssignedToUserId);
        Assert.Equal(updatedAt, experiment.UpdatedAt);
    }

    [Fact]
    public void Unassign_ShouldClearAssignedUser()
    {
        var experiment = CreateExperiment();

        experiment.AssignTo("user-002", Now.AddMinutes(10));
        experiment.Unassign(Now.AddMinutes(20));

        Assert.Null(experiment.AssignedToUserId);
        Assert.Equal(Now.AddMinutes(20), experiment.UpdatedAt);
    }

    [Fact]
    public void Start_ShouldChangeStatusToInProgress()
    {
        var experiment = CreateExperiment();
        var startedAt = Now.AddHours(1);

        experiment.Start(startedAt);

        Assert.Equal(ExperimentStatus.InProgress, experiment.Status);
        Assert.Equal(startedAt, experiment.StartedAt);
        Assert.Equal(startedAt, experiment.UpdatedAt);
    }

    [Fact]
    public void Complete_ShouldChangeStatusToCompleted()
    {
        var experiment = CreateExperiment();
        var startedAt = Now.AddHours(1);
        var completedAt = Now.AddHours(2);

        experiment.Start(startedAt);
        experiment.Complete(completedAt);

        Assert.Equal(ExperimentStatus.Completed, experiment.Status);
        Assert.Equal(startedAt, experiment.StartedAt);
        Assert.Equal(completedAt, experiment.CompletedAt);
        Assert.Equal(completedAt, experiment.UpdatedAt);
    }

    [Fact]
    public void Complete_ShouldRejectCompletionBeforeStart()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddHours(2));

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Complete(Now.AddHours(1)));
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var experiment = CreateExperiment();
        var cancelledAt = Now.AddHours(1);

        experiment.Cancel("Equipment unavailable", cancelledAt);

        Assert.Equal(ExperimentStatus.Cancelled, experiment.Status);
        Assert.Equal(cancelledAt, experiment.UpdatedAt);
    }

    [Fact]
    public void Cancel_ShouldRejectEmptyReason()
    {
        var experiment = CreateExperiment();

        Assert.Throws<ExperimentDomainException>(() =>
            experiment.Cancel(" ", Now.AddHours(1)));
    }

    [Fact]
    public void Archive_ShouldChangeCompletedExperimentToArchived()
    {
        var experiment = CreateExperiment();

        experiment.Start(Now.AddHours(1));
        experiment.Complete(Now.AddHours(2));
        experiment.Archive(Now.AddHours(3));

        Assert.Equal(ExperimentStatus.Archived, experiment.Status);
        Assert.Equal(Now.AddHours(3), experiment.ArchivedAt);
    }

    [Fact]
    public void Archive_ShouldChangeCancelledExperimentToArchived()
    {
        var experiment = CreateExperiment();

        experiment.Cancel("Cancelled by researcher", Now.AddHours(1));
        experiment.Archive(Now.AddHours(2));

        Assert.Equal(ExperimentStatus.Archived, experiment.Status);
        Assert.Equal(Now.AddHours(2), experiment.ArchivedAt);
    }
}
