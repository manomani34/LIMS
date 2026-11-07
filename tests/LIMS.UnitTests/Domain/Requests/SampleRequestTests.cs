using LIMS.Domain.Requests;
using LIMS.Domain.Requests.Enums;
using LIMS.Domain.Requests.Exceptions;
using LIMS.Domain.Samples.Enums;
using Xunit;

namespace LIMS.UnitTests.Requests;

public sealed class SampleRequestTests
{
    private static readonly DateTime CreatedAt =
        new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    private static SampleRequest CreateRequest()
    {
        return SampleRequest.Create(
            "REQ-001",
            "NMR Analysis Request",
            "Initial request",
            "Saffron Sample",
            "Saffron from batch A",
            SampleType.Powder,
            "Farm A",
            "requester-001",
            SampleRequestPriority.Normal,
            CreatedAt);
    }

    [Fact]
    public void Create_ShouldInitializeDraftRequest()
    {
        var request = CreateRequest();

        Assert.NotEqual(Guid.Empty, request.Id);
        Assert.Equal("REQ-001", request.RequestCode);
        Assert.Equal("NMR Analysis Request", request.Title);
        Assert.Equal("Saffron Sample", request.SampleName);
        Assert.Equal(SampleType.Powder, request.RequestedSampleType);
        Assert.Equal(SampleRequestPriority.Normal, request.Priority);
        Assert.Equal(SampleRequestStatus.Draft, request.Status);
        Assert.Equal("requester-001", request.RequestedByUserId);
        Assert.Equal(CreatedAt, request.CreatedAt);
    }

    [Fact]
    public void Create_ShouldNormalizeRequestCode()
    {
        var request = SampleRequest.Create(
            "  req-002  ",
            "Analysis",
            null,
            "Sample",
            null,
            SampleType.Solid,
            null,
            "requester-001",
            SampleRequestPriority.High,
            CreatedAt);

        Assert.Equal("REQ-002", request.RequestCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid code")]
    [InlineData("req/001")]
    public void Create_ShouldRejectInvalidRequestCode(string code)
    {
        Assert.Throws<SampleRequestDomainException>(() =>
            SampleRequest.Create(
                code,
                "Analysis",
                null,
                "Sample",
                null,
                SampleType.Solid,
                null,
                "requester-001",
                SampleRequestPriority.Normal,
                CreatedAt));
    }

    [Fact]
    public void Create_ShouldRejectEmptyTitle()
    {
        Assert.Throws<SampleRequestDomainException>(() =>
            SampleRequest.Create(
                "REQ-001",
                "",
                null,
                "Sample",
                null,
                SampleType.Solid,
                null,
                "requester-001",
                SampleRequestPriority.Normal,
                CreatedAt));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateDraftRequest()
    {
        var request = CreateRequest();
        var updatedAt = CreatedAt.AddHours(1);

        request.UpdateInformation(
            "Updated title",
            "Updated description",
            "Updated sample",
            "Updated sample description",
            SampleType.Liquid,
            "Updated source",
            SampleRequestPriority.High,
            updatedAt);

        Assert.Equal("Updated title", request.Title);
        Assert.Equal("Updated description", request.Description);
        Assert.Equal("Updated sample", request.SampleName);
        Assert.Equal(SampleType.Liquid, request.RequestedSampleType);
        Assert.Equal(SampleRequestPriority.High, request.Priority);
        Assert.Equal(updatedAt, request.UpdatedAt);
    }

    [Fact]
    public void Submit_ShouldChangeStatusToSubmitted()
    {
        var request = CreateRequest();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        Assert.Equal(SampleRequestStatus.Submitted, request.Status);
        Assert.Equal("requester-001", request.RequestedByUserId);
        Assert.NotNull(request.SubmittedAt);
    }

    [Fact]
    public void Submit_ShouldRejectDifferentUser()
    {
        var request = CreateRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Submit(
                "another-user",
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void StartReview_ShouldAssignReviewer()
    {
        var request = CreateSubmittedRequest();

        request.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(2));

        Assert.Equal(SampleRequestStatus.UnderReview, request.Status);
        Assert.Equal("reviewer-001", request.AssignedReviewerUserId);
        Assert.NotNull(request.ReviewStartedAt);
    }

    [Fact]
    public void StartReview_ShouldRejectRequesterAsReviewer()
    {
        var request = CreateSubmittedRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.StartReview(
                "requester-001",
                CreatedAt.AddMinutes(2)));
    }

    [Fact]
    public void Approve_ShouldAssignLaboratory()
    {
        var request = CreateUnderReviewRequest();
        var laboratoryId = Guid.NewGuid();

        request.Approve(
            "reviewer-001",
            laboratoryId,
            "Approved",
            CreatedAt.AddMinutes(3));

        Assert.Equal(SampleRequestStatus.Approved, request.Status);
        Assert.Equal(laboratoryId, request.LaboratoryId);
        Assert.Equal("Approved", request.ReviewComments);
        Assert.NotNull(request.ApprovedAt);
    }

    [Fact]
    public void Approve_ShouldRejectDifferentReviewer()
    {
        var request = CreateUnderReviewRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Approve(
                "reviewer-002",
                Guid.NewGuid(),
                null,
                CreatedAt.AddMinutes(3)));
    }

    [Fact]
    public void Reject_ShouldRecordReason()
    {
        var request = CreateUnderReviewRequest();

        request.Reject(
            "reviewer-001",
            "Insufficient information",
            CreatedAt.AddMinutes(3));

        Assert.Equal(SampleRequestStatus.Rejected, request.Status);
        Assert.Equal(
            "Insufficient information",
            request.RejectionReason);
    }

    [Fact]
    public void Cancel_ShouldAllowRequesterToCancelDraft()
    {
        var request = CreateRequest();

        request.Cancel(
            "requester-001",
            "No longer needed",
            CreatedAt.AddMinutes(1));

        Assert.Equal(SampleRequestStatus.Cancelled, request.Status);
        Assert.Equal("No longer needed", request.CancellationReason);
        Assert.NotNull(request.CancelledAt);
    }

    [Fact]
    public void Cancel_ShouldRejectDifferentUser()
    {
        var request = CreateRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Cancel(
                "another-user",
                null,
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void Cancel_ShouldRejectApprovedRequest()
    {
        var request = CreateApprovedRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Cancel(
                "requester-001",
                "Cancel",
                CreatedAt.AddMinutes(4)));
    }

    [Fact]
    public void Fulfill_ShouldLinkCreatedSample()
    {
        var request = CreateApprovedRequest();
        var sampleId = Guid.NewGuid();

        request.Fulfill(
            sampleId,
            CreatedAt.AddMinutes(4));

        Assert.Equal(SampleRequestStatus.Fulfilled, request.Status);
        Assert.Equal(sampleId, request.CreatedSampleId);
        Assert.NotNull(request.FulfilledAt);
    }

    [Fact]
    public void Fulfill_ShouldRejectEmptySampleId()
    {
        var request = CreateApprovedRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Fulfill(
                Guid.Empty,
                CreatedAt.AddMinutes(4)));
    }

    [Fact]
    public void UpdateInformation_ShouldRejectNonDraftRequest()
    {
        var request = CreateSubmittedRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.UpdateInformation(
                "Updated title",
                null,
                "Sample",
                null,
                SampleType.Solid,
                null,
                SampleRequestPriority.Normal,
                CreatedAt.AddMinutes(3)));
    }

    private static SampleRequest CreateSubmittedRequest()
    {
        var request = CreateRequest();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        return request;
    }

    private static SampleRequest CreateUnderReviewRequest()
    {
        var request = CreateSubmittedRequest();

        request.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(2));

        return request;
    }

    private static SampleRequest CreateApprovedRequest()
    {
        var request = CreateUnderReviewRequest();

        request.Approve(
            "reviewer-001",
            Guid.NewGuid(),
            "Approved",
            CreatedAt.AddMinutes(3));

        return request;
    }
}
