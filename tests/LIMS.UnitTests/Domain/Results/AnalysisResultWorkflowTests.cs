using LIMS.Domain.Results;
using LIMS.Domain.Results.Enums;
using LIMS.Domain.Results.Exceptions;
using Xunit;

namespace LIMS.UnitTests.Results;

public sealed class AnalysisResultWorkflowTests
{
    private static readonly Guid LaboratoryId = Guid.NewGuid();
    private static readonly Guid ExperimentId = Guid.NewGuid();
    private static readonly Guid SampleId = Guid.NewGuid();

    private static readonly DateTime CreatedAt =
        new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    private static AnalysisResult CreateDraft()
    {
        return AnalysisResult.Create(
            LaboratoryId,
            ExperimentId,
            SampleId,
            null,
            "RES-001",
            "NMR Analysis",
            null,
            "creator-001",
            CreatedAt);
    }

    private static AnalysisResult CreateSubmitted()
    {
        var result = CreateDraft();

        AddEntry(result);

        result.Submit(
            "creator-001",
            CreatedAt.AddMinutes(2));

        return result;
    }

    private static AnalysisResult CreateUnderReview()
    {
        var result = CreateSubmitted();

        result.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(3));

        return result;
    }

    private static void AddEntry(AnalysisResult result)
    {
        result.AddEntry(
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            "NMR",
            null,
            CreatedAt.AddMinutes(1));
    }

    [Fact]
    public void Workflow_ShouldSupportCompleteApprovalProcess()
    {
        var result = CreateDraft();

        AddEntry(result);

        Assert.Equal(ResultStatus.Draft, result.Status);

        result.Submit(
            "creator-001",
            CreatedAt.AddMinutes(2));

        Assert.Equal(ResultStatus.Submitted, result.Status);

        result.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(3));

        Assert.Equal(ResultStatus.UnderReview, result.Status);

        result.Approve(
            "reviewer-001",
            "All measurements verified.",
            CreatedAt.AddMinutes(4));

        Assert.Equal(ResultStatus.Approved, result.Status);

        result.Archive(
            "admin-001",
            CreatedAt.AddMinutes(5));

        Assert.Equal(ResultStatus.Archived, result.Status);
    }

    [Fact]
    public void Workflow_ShouldSupportRejectionAndReturnToDraft()
    {
        var result = CreateUnderReview();

        result.Reject(
            "reviewer-001",
            "Additional measurements required.",
            CreatedAt.AddMinutes(4));

        Assert.Equal(ResultStatus.Rejected, result.Status);

        result.ReturnToDraft(
            "creator-001",
            CreatedAt.AddMinutes(5));

        Assert.Equal(ResultStatus.Draft, result.Status);
        Assert.Null(result.SubmittedByUserId);
        Assert.Null(result.ReviewedByUserId);
        Assert.Null(result.RejectionReason);
    }

    [Fact]
    public void Workflow_ShouldAllowRejectedResultToBeArchived()
    {
        var result = CreateUnderReview();

        result.Reject(
            "reviewer-001",
            "Invalid measurement.",
            CreatedAt.AddMinutes(4));

        result.Archive(
            "admin-001",
            CreatedAt.AddMinutes(5));

        Assert.Equal(ResultStatus.Archived, result.Status);
        Assert.Equal("admin-001", result.ArchivedByUserId);
    }

    [Fact]
    public void Workflow_ShouldNotAllowEditingAfterSubmission()
    {
        var result = CreateSubmitted();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.UpdateInformation(
                "Updated title",
                "Updated description",
                null,
                CreatedAt.AddMinutes(3)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowAddingEntriesAfterSubmission()
    {
        var result = CreateSubmitted();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "Temperature",
                ResultValueType.Numeric,
                null,
                25m,
                "C",
                null,
                null,
                null,
                CreatedAt.AddMinutes(3)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowStartingReviewTwice()
    {
        var result = CreateUnderReview();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.StartReview(
                "reviewer-002",
                CreatedAt.AddMinutes(4)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowApprovalBeforeReview()
    {
        var result = CreateSubmitted();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.Approve(
                "reviewer-001",
                null,
                CreatedAt.AddMinutes(3)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowRejectionBeforeReview()
    {
        var result = CreateSubmitted();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.Reject(
                "reviewer-001",
                "Invalid result.",
                CreatedAt.AddMinutes(3)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowReturningApprovedResultToDraft()
    {
        var result = CreateUnderReview();

        result.Approve(
            "reviewer-001",
            null,
            CreatedAt.AddMinutes(4));

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.ReturnToDraft(
                "creator-001",
                CreatedAt.AddMinutes(5)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowArchivingResultTwice()
    {
        var result = CreateUnderReview();

        result.Approve(
            "reviewer-001",
            null,
            CreatedAt.AddMinutes(4));

        result.Archive(
            "admin-001",
            CreatedAt.AddMinutes(5));

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.Archive(
                "admin-001",
                CreatedAt.AddMinutes(6)));
    }

    [Fact]
    public void Workflow_ShouldKeepEntriesWhenReturningRejectedResultToDraft()
    {
        var result = CreateUnderReview();

        result.Reject(
            "reviewer-001",
            "Additional evidence required.",
            CreatedAt.AddMinutes(4));

        result.ReturnToDraft(
            "creator-001",
            CreatedAt.AddMinutes(5));

        Assert.Single(result.Entries);
        Assert.Equal("Chemical Shift", result.Entries.Single().ParameterName);
    }

    [Fact]
    public void Workflow_ShouldPreserveOriginalCreator()
    {
        var result = CreateUnderReview();

        result.Reject(
            "reviewer-001",
            "Additional evidence required.",
            CreatedAt.AddMinutes(4));

        result.ReturnToDraft(
            "creator-001",
            CreatedAt.AddMinutes(5));

        Assert.Equal("creator-001", result.CreatedByUserId);
    }
}
