using LIMS.Application.Common.Dispatching;
using LIMS.Application.Instruments.Commands;
using LIMS.Application.Instruments.Queries;
using LIMS.Contracts.Instruments;
using LIMS.Contracts.Instruments.Mappings;
using LIMS.Contracts.Instruments.Requests;
using LIMS.Contracts.Instruments.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Instruments.Controllers;

[ApiController]
[Area("Instruments")]
[Route("api/v1/instruments")]
[Produces("application/json")]
public sealed class InstrumentsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        var id = await dispatcher.SendAsync(
            new RegisterInstrumentCommand(
                request.LaboratoryId,
                request.InstrumentCode,
                request.Name,
                request.Type.ToDomain(),
                request.RegisteredByUserId,
                request.Manufacturer,
                request.Model,
                request.SerialNumber,
                request.Location,
                request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { instrumentId = id }, new { id });
    }

    [HttpGet("{instrumentId:guid}")]
    [ProducesResponseType(typeof(InstrumentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid instrumentId,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(
            new GetInstrumentByIdQuery(instrumentId),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(ToDetailsResponse(result));
    }

    [HttpGet("by-code/{laboratoryId:guid}/{instrumentCode}")]
    [ProducesResponseType(typeof(InstrumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        Guid laboratoryId,
        string instrumentCode,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(
            new GetInstrumentByCodeQuery(laboratoryId, instrumentCode),
            cancellationToken);

        return result is null ? NotFound() : Ok(ToResponse(result));
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<InstrumentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var results = await dispatcher.QueryAsync(
            new GetInstrumentsByLaboratoryIdQuery(laboratoryId),
            cancellationToken);

        return Ok(results.Select(ToResponse).ToArray());
    }

    [HttpGet("by-status/{laboratoryId:guid}/{status}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<InstrumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByStatus(
        Guid laboratoryId,
        InstrumentStatusContract status,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(status))
        {
            return BadRequest();
        }

        var results = await dispatcher.QueryAsync(
            new GetInstrumentsByStatusQuery(laboratoryId, status.ToDomain()),
            cancellationToken);

        return Ok(results.Select(ToResponse).ToArray());
    }

    [HttpPut("{instrumentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid instrumentId,
        [FromBody] UpdateInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new UpdateInstrumentInformationCommand(
                instrumentId,
                request.Name,
                request.Type.ToDomain(),
                request.Manufacturer,
                request.Model,
                request.SerialNumber,
                request.Location,
                request.Description),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{instrumentId:guid}/use")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkInUse(
        Guid instrumentId,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new MarkInstrumentInUseCommand(instrumentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{instrumentId:guid}/available")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAvailable(
        Guid instrumentId,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new MarkInstrumentAvailableCommand(instrumentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{instrumentId:guid}/maintenance")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScheduleMaintenance(
        Guid instrumentId,
        [FromBody] ScheduleInstrumentMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        var maintenanceId = await dispatcher.SendAsync(
            new ScheduleInstrumentMaintenanceCommand(
                instrumentId,
                request.Type.ToDomain(),
                request.Description,
                request.ScheduledAt,
                request.ScheduledByUserId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { instrumentId },
            new { maintenanceId });
    }

    [HttpPost("{instrumentId:guid}/maintenance/{maintenanceId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteMaintenance(
        Guid instrumentId,
        Guid maintenanceId,
        [FromBody] CompleteInstrumentMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new CompleteInstrumentMaintenanceCommand(
                instrumentId,
                maintenanceId,
                request.CompletedByUserId,
                request.CompletionNotes),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{instrumentId:guid}/out-of-service")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkOutOfService(
        Guid instrumentId,
        [FromBody] MarkInstrumentOutOfServiceRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new MarkInstrumentOutOfServiceCommand(instrumentId, request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{instrumentId:guid}/restore-service")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreService(
        Guid instrumentId,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new RestoreInstrumentServiceCommand(instrumentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{instrumentId:guid}/retire")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Retire(
        Guid instrumentId,
        CancellationToken cancellationToken)
    {
        var updated = await dispatcher.SendAsync(
            new RetireInstrumentCommand(instrumentId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    private static InstrumentResponse ToResponse(
        LIMS.Application.Instruments.DTOs.InstrumentDto dto)
    {
        return new InstrumentResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.InstrumentCode,
            dto.Name,
            dto.Manufacturer,
            dto.Model,
            dto.SerialNumber,
            dto.Type.ToContract(),
            dto.Status.ToContract(),
            dto.Location,
            dto.Description,
            dto.RegisteredAt,
            dto.UpdatedAt,
            dto.RetiredAt);
    }

    private static InstrumentDetailsResponse ToDetailsResponse(
        LIMS.Application.Instruments.DTOs.InstrumentDetailsDto dto)
    {
        return new InstrumentDetailsResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.InstrumentCode,
            dto.Name,
            dto.Manufacturer,
            dto.Model,
            dto.SerialNumber,
            dto.Type.ToContract(),
            dto.Status.ToContract(),
            dto.Location,
            dto.Description,
            dto.RegisteredAt,
            dto.UpdatedAt,
            dto.RetiredAt,
            dto.MaintenanceRecords
                .Select(x => new InstrumentMaintenanceResponse(
                    x.Id,
                    x.InstrumentId,
                    x.Type.ToContract(),
                    x.Description,
                    x.ScheduledAt,
                    x.ScheduledByUserId,
                    x.CompletedAt,
                    x.CompletedByUserId,
                    x.CompletionNotes))
                .ToArray());
    }
}
