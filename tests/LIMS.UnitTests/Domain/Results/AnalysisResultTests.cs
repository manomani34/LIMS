using LIMS.Domain.Results;
using LIMS.Domain.Results.Enums;
using Xunit;

namespace LIMS.UnitTests.Results;

public sealed class AnalysisResultTests
{
    private static readonly Guid LaboratoryId =
        Guid.NewGuid();

    private static readonly Guid ExperimentId =
        Guid.NewGuid();

    private static readonly Guid SampleId =
        Guid.NewGuid();

    private static readonly DateTime CreatedAt =
        new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    private static AnalysisResult CreateResult()
    {
        return AnalysisResult.Create(
            LaboratoryId,
            ExperimentId,
            SampleId,
            null,
            "RES-001",
            "NMR Analysis",
            "Initial analysis",
            "user-001",
            CreatedAt);
    }

    [Fact]
    public void Create_ShouldInitializeResultInDraftStatus()
    {
        var result = CreateResult();

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(LaboratoryId, result.LaboratoryId);
        Assert.Equal(ExperimentId, result.ExperimentId);
        Assert.Equal(SampleId, result.SampleId);
        Assert.Equal("RES-001", result.ResultCode);
        Assert.Equal("NMR Analysis", result.Title);
        Assert.Equal(ResultStatus.Draft, result.Status);
        Assert.Empty(result.Entries);
        Assert.Equal(CreatedAt, result.CreatedAt);
    }

    [Fact]
    public void Create_ShouldRejectEmptyLaboratoryId()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            AnalysisResult.Create(
                Guid.Empty,
                ExperimentId,
                SampleId,
                null,
                "RES-001",
                "NMR Analysis",
                null,
                "user-001",
                CreatedAt));
    }

    [Fact]
    public void Create_ShouldRejectEmptyExperimentId()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            AnalysisResult.Create(
                LaboratoryId,
                Guid.Empty,
                SampleId,
                null,
                "RES-001",
                "NMR Analysis",
                null,
                "user-001",
                CreatedAt));
    }

    [Fact]
    public void Create_ShouldRejectEmptySampleId()
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            AnalysisResult.Create(
                LaboratoryId,
                ExperimentId,
                Guid.Empty,
                null,
                "RES-001",
                "NMR Analysis",
                null,
                "user-001",
                CreatedAt));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid code")]
    [InlineData("res-001")]
    public void Create_ShouldRejectInvalidResultCode(string resultCode)
    {
        Assert.Throws<AnalysisResultDomainException>(() =>
            AnalysisResult.Create(
                LaboratoryId,
                ExperimentId,
                SampleId,
                null,
                resultCode,
                "NMR Analysis",
                null,
                "user-001",
                CreatedAt));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateDraftResult()
    {
        var result = CreateResult();
        var updatedAt = CreatedAt.AddHours(1);

        result.UpdateInformation(
            "Updated NMR Analysis",
            "Updated description",
            null,
            updatedAt);

        Assert.Equal("Updated NMR Analysis", result.Title);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal(updatedAt, result.UpdatedAt);
    }

    [Fact]
    public void AddEntry_ShouldAddEntryAndReturnItsId()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            "NMR",
            null,
            CreatedAt.AddMinutes(1));

        Assert.NotEqual(Guid.Empty, entryId);
        Assert.Single(result.Entries);
        Assert.Equal(entryId, result.Entries.Single().Id);
        Assert.Equal("Chemical Shift", result.Entries.Single().ParameterName);
    }

    [Fact]
    public void AddEntry_ShouldRejectDuplicateParameterName()
    {
        var result = CreateResult();

        result.AddEntry(
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.AddEntry(
                "chemical shift",
                ResultValueType.Numeric,
                null,
                8.10m,
                "ppm",
                null,
                null,
                null,
                CreatedAt.AddMinutes(2)));
    }

    [Fact]
    public void RemoveEntry_ShouldRemoveExistingEntry()
    {
        var result = CreateResult();

        var entryId = result.AddEntry(
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        result.RemoveEntry(
            entryId,
            CreatedAt.AddMinutes(2));

        Assert.Empty(result.Entries);
    }

    [Fact]
    public void Submit_ShouldRequireAtLeastOneEntry()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.Submit(
                "user-001",
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void Submit_ShouldChangeStatusToSubmitted()
    {
        var result = CreateResult();

        result.AddEntry(
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        result.Submit(
            "user-001",
            CreatedAt.AddMinutes(2));

        Assert.Equal(ResultStatus.Submitted, result.Status);
        Assert.Equal("user-001", result.SubmittedByUserId);
        Assert.NotNull(result.SubmittedAt);
    }

    [Fact]
    public void StartReview_ShouldAssignReviewer()
    {
        var result = CreateSubmittedResult();

        result.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(3));

        Assert.Equal(ResultStatus.UnderReview, result.Status);
        Assert.Equal("reviewer-001", result.ReviewedByUserId);
    }

    [Fact]
    public void StartReview_ShouldRejectSubmitterAsReviewer()
    {
        var result = CreateSubmittedResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.StartReview(
                "user-001",
                CreatedAt.AddMinutes(3)));
    }

    [Fact]
    public void Approve_ShouldApproveResultAssignedToReviewer()
    {
        var result = CreateUnderReviewResult();

        result.Approve(
            "reviewer-001",
            "Approved",
            CreatedAt.AddMinutes(4));

        Assert.Equal(ResultStatus.Approved, result.Status);
        Assert.Equal("reviewer-001", result.ReviewedByUserId);
        Assert.Equal("Approved", result.ReviewComments);
        Assert.NotNull(result.ReviewedAt);
    }

    [Fact]
    public void Approve_ShouldRejectDifferentReviewer()
    {
        var result = CreateUnderReviewResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.Approve(
                "reviewer-002",
                null,
                CreatedAt.AddMinutes(4)));
    }

    [Fact]
    public void Reject_ShouldRejectResultAssignedToReviewer()
    {
        var result = CreateUnderReviewResult();

        result.Reject(
            "reviewer-001",
            "Insufficient evidence",
            CreatedAt.AddMinutes(4));

        Assert.Equal(ResultStatus.Rejected, result.Status);
        Assert.Equal("Insufficient evidence", result.RejectionReason);
    }

    [Fact]
    public void ReturnToDraft_ShouldAllowCreatorToReviseRejectedResult()
    {
        var result = CreateRejectedResult();

        result.ReturnToDraft(
            "user-001",
            CreatedAt.AddMinutes(5));

        Assert.Equal(ResultStatus.Draft, result.Status);
        Assert.Null(result.SubmittedByUserId);
        Assert.Null(result.ReviewedByUserId);
        Assert.Null(result.SubmittedAt);
        Assert.Null(result.ReviewedAt);
    }

    [Fact]
    public void ReturnToDraft_ShouldRejectNonCreator()
    {
        var result = CreateRejectedResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.ReturnToDraft(
                "another-user",
                CreatedAt.AddMinutes(5)));
    }

    [Fact]
    public void Archive_ShouldArchiveApprovedResult()
    {
        var result = CreateApprovedResult();

        result.Archive(
            "admin-001",
            CreatedAt.AddMinutes(5));

        Assert.Equal(ResultStatus.Archived, result.Status);
        Assert.Equal("admin-001", result.ArchivedByUserId);
        Assert.NotNull(result.ArchivedAt);
    }

    [Fact]
    public void Archive_ShouldRejectDraftResult()
    {
        var result = CreateResult();

        Assert.Throws<AnalysisResultDomainException>(() =>
            result.Archive(
                "admin-001",
                CreatedAt.AddMinutes(1)));
    }

    private static AnalysisResult CreateSubmittedResult()
    {
        var result = CreateResult();

        result.AddEntry(
            "Chemical Shift",
            ResultValueType.Numeric,
            null,
            7.25m,
            "ppm",
            null,
            null,
            null,
            CreatedAt.AddMinutes(1));

        result.Submit(
            "user-001",
            CreatedAt.AddMinutes(2));

        return result;
    }

    private static AnalysisResult CreateUnderReviewResult()
    {
        var result = CreateSubmittedResult();

        result.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(3));

        return result;
    }

    private static AnalysisResult CreateRejectedResult()
    {
        var result = CreateUnderReviewResult();

        result.Reject(
            "reviewer-001",
            "Insufficient evidence",
            CreatedAt.AddMinutes(4));

        return result;
    }

    private static AnalysisResult CreateApprovedResult()
    {
        var result = CreateUnderReviewResult();

        result.Approve(
            "reviewer-001",
            "Approved",
            CreatedAt.AddMinutes(4));

        return result;
    }
}
