
using LIMS.Api.Areas.Samples.Mappings;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Samples.Commands;
using LIMS.Application.Samples.DTOs;
using LIMS.Application.Samples.Queries;
using LIMS.Contracts.Common;
using LIMS.Contracts.Samples;
using LIMS.Contracts.Samples.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Samples.Controllers;

[Area("Samples")]
[ApiController]
[Route("api/v1/samples")]
[Produces("application/json")]
public sealed class SamplesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public SamplesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResourceIdResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterSampleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterSampleCommand(
            request.LaboratoryId,
            request.SampleCode,
            request.Name,
            request.Description,
            request.Type.ToDomain(),
            request.Priority.ToDomain(),
            request.Source,
            request.RegisteredByUserId);

        var sampleId = await _dispatcher.SendAsync<RegisterSampleCommand, Guid>(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { sampleId },
            new ResourceIdResponse(sampleId));
    }

    [HttpGet("{sampleId:guid}")]
    [ProducesResponseType(typeof(SampleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetSampleByIdQuery,
            SampleDetailsDto?>(
            new GetSampleByIdQuery(sampleId),
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToResponse());
    }

    [HttpGet("by-code/{laboratoryId:guid}/{sampleCode}")]
    [ProducesResponseType(typeof(SampleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        Guid laboratoryId,
        string sampleCode,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetSampleByCodeQuery,
            SampleDto?>(
            new GetSampleByCodeQuery(laboratoryId, sampleCode),
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToResponse());
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<SampleResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetSamplesByLaboratoryIdQuery,
            IReadOnlyCollection<SampleDto>>(
            new GetSamplesByLaboratoryIdQuery(laboratoryId),
            cancellationToken);

        return Ok(result
            .Select(sample => sample.ToResponse())
            .ToArray());
    }

    [HttpGet("by-assigned-user/{userId}")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<SampleResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAssignedUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetSamplesByAssignedUserIdQuery,
            IReadOnlyCollection<SampleDto>>(
            new GetSamplesByAssignedUserIdQuery(userId),
            cancellationToken);

        return Ok(result
            .Select(sample => sample.ToResponse())
            .ToArray());
    }

    [HttpPut("{sampleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid sampleId,
        [FromBody] UpdateSampleRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            UpdateSampleInformationCommand,
            bool>(
            new UpdateSampleInformationCommand(
                sampleId,
                request.Name,
                request.Description,
                request.Source),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/receive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Receive(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<ReceiveSampleCommand, bool>(
            new ReceiveSampleCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/preparation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartPreparation(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            StartSamplePreparationCommand,
            bool>(
            new StartSamplePreparationCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/ready-for-analysis")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkReadyForAnalysis(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            MarkSampleReadyForAnalysisCommand,
            bool>(
            new MarkSampleReadyForAnalysisCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/analysis/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartAnalysis(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            StartSampleAnalysisCommand,
            bool>(
            new StartSampleAnalysisCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/analysis/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteAnalysis(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            CompleteSampleAnalysisCommand,
            bool>(
            new CompleteSampleAnalysisCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(
        Guid sampleId,
        [FromBody] RejectSampleRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<RejectSampleCommand, bool>(
            new RejectSampleCommand(sampleId, request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign(
        Guid sampleId,
        [FromBody] AssignSampleRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<AssignSampleCommand, bool>(
            new AssignSampleCommand(sampleId, request.UserId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{sampleId:guid}/assignment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unassign(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<UnassignSampleCommand, bool>(
            new UnassignSampleCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{sampleId:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<ArchiveSampleCommand, bool>(
            new ArchiveSampleCommand(sampleId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }
}
