
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Results.Commands;
using LIMS.Application.Results.Queries;
using LIMS.Contracts.Common;
using LIMS.Contracts.Results.Mappings;
using LIMS.Contracts.Results.Requests;
using LIMS.Contracts.Results.Responses;
using LIMS.Domain.Results.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Results.Controllers;

[ApiController]
[Area("Results")]
[Route("api/v1/results")]
[Produces("application/json")]
public sealed class ResultsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ResultsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResourceIdResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAnalysisResultRequest request,
        CancellationToken cancellationToken)
    {
        var resultId = await _dispatcher.SendAsync<CreateAnalysisResultCommand, Guid>(
            new CreateAnalysisResultCommand(
                request.LaboratoryId,
                request.ExperimentId,
                request.SampleId,
                request.InstrumentId,
                request.ResultCode,
                request.Title,
                request.Description,
                request.CreatedByUserId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { resultId },
            new ResourceIdResponse(resultId));
    }

    [HttpGet("{resultId:guid}")]
    [ProducesResponseType(typeof(AnalysisResultDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid resultId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetAnalysisResultByIdQuery,
            LIMS.Application.Results.DTOs.AnalysisResultDetailsDto?>(
                new GetAnalysisResultByIdQuery(resultId),
                cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToDetailsResponse());
    }

    [HttpGet("by-code/{laboratoryId:guid}/{resultCode}")]
    [ProducesResponseType(typeof(AnalysisResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        Guid laboratoryId,
        string resultCode,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetAnalysisResultByCodeQuery,
            LIMS.Application.Results.DTOs.AnalysisResultDto?>(
                new GetAnalysisResultByCodeQuery(laboratoryId, resultCode),
                cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToResponse());
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AnalysisResultResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync<
            GetAnalysisResultsByLaboratoryIdQuery,
            IReadOnlyCollection<LIMS.Application.Results.DTOs.AnalysisResultDto>>(
                new GetAnalysisResultsByLaboratoryIdQuery(laboratoryId),
                cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpGet("by-experiment/{experimentId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AnalysisResultResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByExperiment(
        Guid experimentId,
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync<
            GetAnalysisResultsByExperimentIdQuery,
            IReadOnlyCollection<LIMS.Application.Results.DTOs.AnalysisResultDto>>(
                new GetAnalysisResultsByExperimentIdQuery(experimentId),
                cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpGet("by-sample/{sampleId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AnalysisResultResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySample(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync<
            GetAnalysisResultsBySampleIdQuery,
            IReadOnlyCollection<LIMS.Application.Results.DTOs.AnalysisResultDto>>(
                new GetAnalysisResultsBySampleIdQuery(sampleId),
                cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpGet("by-status/{laboratoryId:guid}/{status}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AnalysisResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByStatus(
        Guid laboratoryId,
        string status,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ResultStatus>(status, true, out var parsedStatus) ||
            !Enum.IsDefined(parsedStatus))
        {
            return BadRequest(new ErrorResponse(
                "INVALID_RESULT_STATUS",
                "The specified result status is invalid."));
        }

        var results = await _dispatcher.QueryAsync<
            GetAnalysisResultsByStatusQuery,
            IReadOnlyCollection<LIMS.Application.Results.DTOs.AnalysisResultDto>>(
                new GetAnalysisResultsByStatusQuery(laboratoryId, parsedStatus),
                cancellationToken);

        return Ok(results.Select(x => x.ToResponse()).ToArray());
    }

    [HttpPut("{resultId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid resultId,
        [FromBody] UpdateAnalysisResultRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<UpdateAnalysisResultInformationCommand, bool>(
            new UpdateAnalysisResultInformationCommand(
                resultId,
                request.Title,
                request.Description,
                request.InstrumentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/entries")]
    [ProducesResponseType(typeof(ResourceIdResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddEntry(
        Guid resultId,
        [FromBody] AddResultEntryRequest request,
        CancellationToken cancellationToken)
    {
        var entryId = await _dispatcher.SendAsync<AddResultEntryCommand, Guid>(
            new AddResultEntryCommand(
                resultId,
                request.ParameterName,
                request.ValueType.ToDomain(),
                request.Value,
                request.NumericValue,
                request.Unit,
                request.Uncertainty,
                request.Method,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { resultId },
            new ResourceIdResponse(entryId));
    }

    [HttpPut("{resultId:guid}/entries/{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEntry(
        Guid resultId,
        Guid entryId,
        [FromBody] UpdateResultEntryRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<UpdateResultEntryCommand, bool>(
            new UpdateResultEntryCommand(
                resultId,
                entryId,
                request.ParameterName,
                request.ValueType.ToDomain(),
                request.Value,
                request.NumericValue,
                request.Unit,
                request.Uncertainty,
                request.Method,
                request.Notes),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{resultId:guid}/entries/{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEntry(
        Guid resultId,
        Guid entryId,
        CancellationToken cancellationToken)
    {
        var removed = await _dispatcher.SendAsync<RemoveResultEntryCommand, bool>(
            new RemoveResultEntryCommand(resultId, entryId),
            cancellationToken);

        return removed ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit(
        Guid resultId,
        [FromBody] SubmitAnalysisResultRequest request,
        CancellationToken cancellationToken)
    {
        var submitted = await _dispatcher.SendAsync<SubmitAnalysisResultCommand, bool>(
            new SubmitAnalysisResultCommand(
                resultId,
                request.SubmittedByUserId),
            cancellationToken);

        return submitted ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/start-review")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartReview(
        Guid resultId,
        [FromBody] StartAnalysisResultReviewRequest request,
        CancellationToken cancellationToken)
    {
        var started = await _dispatcher.SendAsync<StartAnalysisResultReviewCommand, bool>(
            new StartAnalysisResultReviewCommand(
                resultId,
                request.ReviewerUserId),
            cancellationToken);

        return started ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(
        Guid resultId,
        [FromBody] ApproveAnalysisResultRequest request,
        CancellationToken cancellationToken)
    {
        var approved = await _dispatcher.SendAsync<ApproveAnalysisResultCommand, bool>(
            new ApproveAnalysisResultCommand(
                resultId,
                request.ReviewerUserId,
                request.Comments),
            cancellationToken);

        return approved ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(
        Guid resultId,
        [FromBody] RejectAnalysisResultRequest request,
        CancellationToken cancellationToken)
    {
        var rejected = await _dispatcher.SendAsync<RejectAnalysisResultCommand, bool>(
            new RejectAnalysisResultCommand(
                resultId,
                request.ReviewerUserId,
                request.Reason),
            cancellationToken);

        return rejected ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/return-to-draft")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReturnToDraft(
        Guid resultId,
        [FromBody] ReturnAnalysisResultToDraftRequest request,
        CancellationToken cancellationToken)
    {
        var returned = await _dispatcher.SendAsync<ReturnAnalysisResultToDraftCommand, bool>(
            new ReturnAnalysisResultToDraftCommand(
                resultId,
                request.RequestedByUserId),
            cancellationToken);

        return returned ? NoContent() : NotFound();
    }

    [HttpPost("{resultId:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Archive(
        Guid resultId,
        [FromBody] ArchiveAnalysisResultRequest request,
        CancellationToken cancellationToken)
    {
        var archived = await _dispatcher.SendAsync<ArchiveAnalysisResultCommand, bool>(
            new ArchiveAnalysisResultCommand(
                resultId,
                request.ArchivedByUserId),
            cancellationToken);

        return archived ? NoContent() : NotFound();
    }
}
