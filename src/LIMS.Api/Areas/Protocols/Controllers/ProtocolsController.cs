using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Protocols.Commands;
using LIMS.Application.Protocols.DTOs;
using LIMS.Application.Protocols.Queries;
using LIMS.Contracts.Common;
using LIMS.Contracts.Protocols;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Protocols.Controllers;

[ApiController]
[Area("Protocols")]
[Route("api/v1/protocols")]
[Produces("application/json")]
public sealed class ProtocolsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ProtocolsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProtocolRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _dispatcher.SendAsync<CreateProtocolCommand, Guid>(new CreateProtocolCommand(
                request.LaboratoryId,
                request.ProtocolCode,
                request.Name,
                request.Description,
                request.Version,
                request.CreatedByUserId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { protocolId = id },
            id);
    }

    [HttpGet("{protocolId:guid}")]
    [ProducesResponseType(typeof(ProtocolDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid protocolId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<GetProtocolByIdQuery, LIMS.Application.Protocols.DTOs.ProtocolDetailsDto?>(new GetProtocolByIdQuery(protocolId),
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(Map(result));
    }

    [HttpGet("by-code/{laboratoryId:guid}/{protocolCode}")]
    [ProducesResponseType(typeof(ProtocolResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        Guid laboratoryId,
        string protocolCode,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<GetProtocolByCodeQuery, LIMS.Application.Protocols.DTOs.ProtocolDto?>(new GetProtocolByCodeQuery(laboratoryId, protocolCode),
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(Map(result));
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProtocolResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var results = await _dispatcher.QueryAsync<GetProtocolsByLaboratoryIdQuery, IReadOnlyCollection<LIMS.Application.Protocols.DTOs.ProtocolDto>>(new GetProtocolsByLaboratoryIdQuery(laboratoryId),
            cancellationToken);

        return Ok(results.Select(Map).ToArray());
    }

    [HttpPut("{protocolId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid protocolId,
        [FromBody] UpdateProtocolRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<UpdateProtocolInformationCommand, bool>(new UpdateProtocolInformationCommand(
                protocolId,
                request.Name,
                request.Description,
                request.Version),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{protocolId:guid}/steps")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddStep(
        Guid protocolId,
        [FromBody] AddProtocolStepRequest request,
        CancellationToken cancellationToken)
    {
        var stepId = await _dispatcher.SendAsync<AddProtocolStepCommand, Guid>(new AddProtocolStepCommand(
                protocolId,
                request.Sequence,
                request.Title,
                request.Instruction,
                request.Type.ToDomain(),
                request.IsRequired,
                request.TargetValue,
                request.TargetUnit,
                request.MinimumValue,
                request.MaximumValue,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { protocolId },
            stepId);
    }

    [HttpPut("{protocolId:guid}/steps/{stepId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStep(
        Guid protocolId,
        Guid stepId,
        [FromBody] UpdateProtocolStepRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<UpdateProtocolStepCommand, bool>(new UpdateProtocolStepCommand(
                protocolId,
                stepId,
                request.Sequence,
                request.Title,
                request.Instruction,
                request.Type.ToDomain(),
                request.IsRequired,
                request.TargetValue,
                request.TargetUnit,
                request.MinimumValue,
                request.MaximumValue,
                request.Notes),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{protocolId:guid}/steps/{stepId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveStep(
        Guid protocolId,
        Guid stepId,
        CancellationToken cancellationToken)
    {
        var removed = await _dispatcher.SendAsync<RemoveProtocolStepCommand, bool>(new RemoveProtocolStepCommand(protocolId, stepId),
            cancellationToken);

        return removed ? NoContent() : NotFound();
    }

    [HttpPost("{protocolId:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate(
        Guid protocolId,
        CancellationToken cancellationToken)
    {
        var activated = await _dispatcher.SendAsync<ActivateProtocolCommand, bool>(new ActivateProtocolCommand(protocolId),
            cancellationToken);

        return activated ? NoContent() : NotFound();
    }

    [HttpPost("{protocolId:guid}/deprecate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deprecate(
        Guid protocolId,
        CancellationToken cancellationToken)
    {
        var deprecated = await _dispatcher.SendAsync<DeprecateProtocolCommand, bool>(new DeprecateProtocolCommand(protocolId),
            cancellationToken);

        return deprecated ? NoContent() : NotFound();
    }

    [HttpPost("{protocolId:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Archive(
        Guid protocolId,
        CancellationToken cancellationToken)
    {
        var archived = await _dispatcher.SendAsync<ArchiveProtocolCommand, bool>(new ArchiveProtocolCommand(protocolId),
            cancellationToken);

        return archived ? NoContent() : NotFound();
    }

    private static ProtocolResponse Map(ProtocolDto dto)
    {
        return new ProtocolResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.ProtocolCode,
            dto.Name,
            dto.Description,
            dto.Version,
            (ProtocolStatusContract)(int)dto.Status,
            dto.CreatedByUserId,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.ActivatedAt,
            dto.DeprecatedAt,
            dto.ArchivedAt);
    }

    private static ProtocolDetailsResponse Map(ProtocolDetailsDto dto)
    {
        return new ProtocolDetailsResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.ProtocolCode,
            dto.Name,
            dto.Description,
            dto.Version,
            (ProtocolStatusContract)(int)dto.Status,
            dto.CreatedByUserId,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.ActivatedAt,
            dto.DeprecatedAt,
            dto.ArchivedAt,
            dto.Steps.Select(step => new ProtocolStepResponse(
                step.Id,
                step.ProtocolId,
                step.Sequence,
                step.Title,
                step.Instruction,
                (ProtocolStepTypeContract)(int)step.Type,
                step.IsRequired,
                step.TargetValue,
                step.TargetUnit,
                step.MinimumValue,
                step.MaximumValue,
                step.Notes)).ToArray());
    }
}
