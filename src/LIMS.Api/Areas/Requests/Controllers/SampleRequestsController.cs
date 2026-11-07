using LIMS.Application.Abstractions.Identity;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Requests.Queries;
using LIMS.Contracts.Common;
using LIMS.Contracts.Requests.Mapping;
using LIMS.Contracts.Requests.Requests;
using LIMS.Contracts.Requests.Responses;
using LIMS.Domain.Requests.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Requests.Controllers;

[Area("Requests")]
[ApiController]
[Route("api/v1/sample-requests")]
[Produces("application/json")]
public sealed class SampleRequestsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly ICurrentUser _currentUser;

    public SampleRequestsController(
        IDispatcher dispatcher,
        ICurrentUser currentUser)
    {
        _dispatcher = dispatcher;
        _currentUser = currentUser;
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSampleRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSampleRequestCommand(
            request.RequestCode,
            request.Title,
            request.Description,
            request.SampleName,
            request.SampleDescription,
            request.RequestedSampleType.ToDomain(),
            request.SampleSource,
            RequireCurrentUserId(),
            request.Priority.ToDomain());

        var requestId = await _dispatcher.SendAsync(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { requestId },
            new { id = requestId });
    }

    [HttpGet("{requestId:guid}")]
    [ProducesResponseType(typeof(SampleRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetSampleRequestByIdQuery(requestId),
            cancellationToken);

        return result is null ? NotFound() : Ok(result.ToResponse());
    }

    [HttpGet("by-code/{requestCode}")]
    [ProducesResponseType(typeof(SampleRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        string requestCode,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetSampleRequestByCodeQuery(requestCode),
            cancellationToken);

        return result is null ? NotFound() : Ok(result.ToResponse());
    }

    [HttpGet("by-requester/{userId}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SampleRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRequester(
        string userId,
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync(
            new GetSampleRequestsByRequesterQuery(userId),
            cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SampleRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync(
            new GetSampleRequestsByLaboratoryQuery(laboratoryId),
            cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpGet("by-status/{status}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SampleRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByStatus(
        string status,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<SampleRequestStatus>(
                status,
                true,
                out var parsedStatus)
            || !Enum.IsDefined(parsedStatus))
        {
            return BadRequest(new ErrorResponse(
                "invalid_request_status",
                "The supplied request status is invalid.",
                HttpContext.TraceIdentifier));
        }

        var results = await _dispatcher.QueryAsync(
            new GetSampleRequestsByStatusQuery(parsedStatus),
            cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpGet("pending-review")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SampleRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingReview(
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync(
            new GetPendingSampleRequestsQuery(),
            cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpPut("{requestId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid requestId,
        [FromBody] UpdateSampleRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSampleRequestCommand(
            requestId,
            request.Title,
            request.Description,
            request.SampleName,
            request.SampleDescription,
            request.RequestedSampleType.ToDomain(),
            request.SampleSource,
            request.Priority.ToDomain());

        var updated = await _dispatcher.SendAsync(
            command,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Submit(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new SubmitSampleRequestCommand(
                requestId,
                RequireCurrentUserId()),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/start-review")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartReview(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new StartSampleRequestReviewCommand(
                requestId,
                RequireCurrentUserId()),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
        Guid requestId,
        [FromBody] ApproveSampleRequestRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new ApproveSampleRequestCommand(
                requestId,
                RequireCurrentUserId(),
                request.LaboratoryId,
                request.Comments),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        Guid requestId,
        [FromBody] RejectSampleRequestRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new RejectSampleRequestCommand(
                requestId,
                RequireCurrentUserId(),
                request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid requestId,
        [FromBody] CancelSampleRequestRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new CancelSampleRequestCommand(
                requestId,
                RequireCurrentUserId(),
                request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/fulfill")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Fulfill(
        Guid requestId,
        [FromQuery] Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new FulfillSampleRequestCommand(requestId, sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    private string RequireCurrentUserId()
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            throw new InvalidOperationException(
                "An authenticated user is required for this operation.");

        return _currentUser.UserId;
    }
}
