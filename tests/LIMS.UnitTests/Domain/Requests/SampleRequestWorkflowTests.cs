using LIMS.Domain.Requests;
using LIMS.Domain.Requests.Enums;
using LIMS.Domain.Requests.Exceptions;
using LIMS.Domain.Samples.Enums;
using Xunit;

namespace LIMS.UnitTests.Requests;

public sealed class SampleRequestWorkflowTests
{
    private static readonly DateTime CreatedAt =
        new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    private static SampleRequest CreateRequest()
    {
        return SampleRequest.Create(
            "REQ-WORKFLOW-001",
            "NMR Analysis",
            null,
            "Saffron",
            null,
            SampleType.Powder,
            "Farm A",
            "requester-001",
            SampleRequestPriority.High,
            CreatedAt);
    }

    [Fact]
    public void Workflow_ShouldSupportApprovalAndFulfillment()
    {
        var request = CreateRequest();
        var laboratoryId = Guid.NewGuid();
        var sampleId = Guid.NewGuid();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        request.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(2));

        request.Approve(
            "reviewer-001",
            laboratoryId,
            "Approved",
            CreatedAt.AddMinutes(3));

        request.Fulfill(
            sampleId,
            CreatedAt.AddMinutes(4));

        Assert.Equal(SampleRequestStatus.Fulfilled, request.Status);
        Assert.Equal(laboratoryId, request.LaboratoryId);
        Assert.Equal(sampleId, request.CreatedSampleId);
    }

    [Fact]
    public void Workflow_ShouldSupportRejection()
    {
        var request = CreateRequest();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        request.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(2));

        request.Reject(
            "reviewer-001",
            "Insufficient sample information",
            CreatedAt.AddMinutes(3));

        Assert.Equal(SampleRequestStatus.Rejected, request.Status);
        Assert.Equal(
            "Insufficient sample information",
            request.RejectionReason);
    }

    [Fact]
    public void Workflow_ShouldNotAllowApprovalBeforeReview()
    {
        var request = CreateRequest();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Approve(
                "reviewer-001",
                Guid.NewGuid(),
                null,
                CreatedAt.AddMinutes(2)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowFulfillmentBeforeApproval()
    {
        var request = CreateRequest();

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Fulfill(
                Guid.NewGuid(),
                CreatedAt.AddMinutes(1)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowReviewAfterRejection()
    {
        var request = CreateRequest();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        request.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(2));

        request.Reject(
            "reviewer-001",
            "Invalid request",
            CreatedAt.AddMinutes(3));

        Assert.Throws<SampleRequestDomainException>(() =>
            request.StartReview(
                "reviewer-002",
                CreatedAt.AddMinutes(4)));
    }

    [Fact]
    public void Workflow_ShouldNotAllowCancellationAfterFulfillment()
    {
        var request = CreateRequest();

        request.Submit(
            "requester-001",
            CreatedAt.AddMinutes(1));

        request.StartReview(
            "reviewer-001",
            CreatedAt.AddMinutes(2));

        request.Approve(
            "reviewer-001",
            Guid.NewGuid(),
            null,
            CreatedAt.AddMinutes(3));

        request.Fulfill(
            Guid.NewGuid(),
            CreatedAt.AddMinutes(4));

        Assert.Throws<SampleRequestDomainException>(() =>
            request.Cancel(
                "requester-001",
                "Cancel",
                CreatedAt.AddMinutes(5)));
    }
}
