using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Experiments.Commands;
using LIMS.Application.Experiments.DTOs;
using LIMS.Application.Experiments.Queries;
using LIMS.Contracts.Experiments.Requests;
using LIMS.Contracts.Experiments.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Experiments.Controllers;

[Area("Experiments")]
[ApiController]
[Route("api/v1/experiments")]
[Produces("application/json")]
public sealed class ExperimentsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ExperimentsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateExperimentRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _dispatcher.SendAsync(
            new CreateExperimentCommand(
                request.LaboratoryId,
                request.SampleId,
                request.ExperimentCode,
                request.Title,
                request.Description,
                request.CreatedByUserId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { experimentId = id },
            id);
    }

    [HttpGet("{experimentId:guid}")]
    [ProducesResponseType(typeof(ExperimentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid experimentId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetExperimentByIdQuery,
            ExperimentDetailsDto?>(
            new GetExperimentByIdQuery(experimentId),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(ToDetailsResponse(result));
    }

    [HttpGet("by-code/{laboratoryId:guid}/{experimentCode}")]
    [ProducesResponseType(typeof(ExperimentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        Guid laboratoryId,
        string experimentCode,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetExperimentByCodeQuery,
            ExperimentDto?>(
            new GetExperimentByCodeQuery(laboratoryId, experimentCode),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(ToResponse(result));
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ExperimentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetExperimentsByLaboratoryIdQuery,
            IReadOnlyCollection<ExperimentDto>>(
            new GetExperimentsByLaboratoryIdQuery(laboratoryId),
            cancellationToken);

        return Ok(result.Select(ToResponse).ToArray());
    }

    [HttpGet("by-sample/{sampleId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ExperimentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySample(
        Guid sampleId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetExperimentsBySampleIdQuery,
            IReadOnlyCollection<ExperimentDto>>(
            new GetExperimentsBySampleIdQuery(sampleId),
            cancellationToken);

        return Ok(result.Select(ToResponse).ToArray());
    }

    [HttpGet("by-assigned-user/{userId}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ExperimentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAssignedUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetExperimentsByAssignedUserIdQuery,
            IReadOnlyCollection<ExperimentDto>>(
            new GetExperimentsByAssignedUserIdQuery(userId),
            cancellationToken);

        return Ok(result.Select(ToResponse).ToArray());
    }

    [HttpPut("{experimentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid experimentId,
        [FromBody] UpdateExperimentRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new UpdateExperimentInformationCommand(
                experimentId,
                request.Title,
                request.Description),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{experimentId:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign(
        Guid experimentId,
        [FromBody] AssignExperimentRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new AssignExperimentCommand(experimentId, request.UserId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{experimentId:guid}/assignment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unassign(
        Guid experimentId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new UnassignExperimentCommand(experimentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{experimentId:guid}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Start(
        Guid experimentId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new StartExperimentCommand(experimentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{experimentId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(
        Guid experimentId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new CompleteExperimentCommand(experimentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{experimentId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(
        Guid experimentId,
        [FromBody] CancelExperimentRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new CancelExperimentCommand(experimentId, request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{experimentId:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(
        Guid experimentId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync(
            new ArchiveExperimentCommand(experimentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    private static ExperimentResponse ToResponse(ExperimentDto dto)
    {
        return new ExperimentResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.SampleId,
            dto.ExperimentCode,
            dto.Title,
            dto.Description,
            dto.Status.ToString(),
            dto.CreatedByUserId,
            dto.AssignedToUserId,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.StartedAt,
            dto.CompletedAt,
            dto.ArchivedAt);
    }

    private static ExperimentDetailsResponse ToDetailsResponse(
        ExperimentDetailsDto dto)
    {
        return new ExperimentDetailsResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.SampleId,
            dto.ExperimentCode,
            dto.Title,
            dto.Description,
            dto.Status.ToString(),
            dto.CreatedByUserId,
            dto.AssignedToUserId,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.StartedAt,
            dto.CompletedAt,
            dto.ArchivedAt);
    }
}
