using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Laboratories.Commands;
using LIMS.Application.Laboratories.DTOs;
using LIMS.Application.Laboratories.Queries;
using LIMS.Contracts.Common;
using LIMS.Contracts.Laboratories;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Laboratories.Controllers;

[Area("Laboratories")]
[ApiController]
[Route("api/v1/laboratories")]
[Produces("application/json")]
public sealed class LaboratoriesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public LaboratoriesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLaboratoryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _dispatcher.SendAsync<CreateLaboratoryCommand, Guid>(
            new CreateLaboratoryCommand(
                request.Name,
                request.Code,
                request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { laboratoryId = id }, new { id });
    }

    [HttpGet("{laboratoryId:guid}")]
    [ProducesResponseType(typeof(LaboratoryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid laboratoryId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetLaboratoryByIdQuery,
            LaboratoryDetailsDto?>(
            new GetLaboratoryByIdQuery(laboratoryId),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(LaboratoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetLaboratoryByCodeQuery,
            LaboratoryDto?>(
            new GetLaboratoryByCodeQuery(code),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-user/{userId}")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<LaboratoryDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetLaboratoriesByUserIdQuery,
            IReadOnlyCollection<LaboratoryDto>>(
            new GetLaboratoriesByUserIdQuery(userId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{laboratoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid laboratoryId,
        [FromBody] UpdateLaboratoryRequest request,
        CancellationToken cancellationToken)
    {
        await _dispatcher.SendAsync<UpdateLaboratoryInformationCommand, bool>(
            new UpdateLaboratoryInformationCommand(
                laboratoryId,
                request.Name,
                request.Code,
                request.Description),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{laboratoryId:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(
        Guid laboratoryId,
        [FromBody] AddLaboratoryMemberRequest request,
        CancellationToken cancellationToken)
    {
        await _dispatcher.SendAsync<AddLaboratoryMemberCommand, bool>(
            new AddLaboratoryMemberCommand(
                laboratoryId,
                request.UserId,
                request.Role),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{laboratoryId:guid}/members/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(
        Guid laboratoryId,
        string userId,
        CancellationToken cancellationToken)
    {
        await _dispatcher.SendAsync<RemoveLaboratoryMemberCommand, bool>(
            new RemoveLaboratoryMemberCommand(laboratoryId, userId),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{laboratoryId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(
        Guid laboratoryId,
        [FromBody] ChangeLaboratoryStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _dispatcher.SendAsync<ChangeLaboratoryStatusCommand, bool>(
            new ChangeLaboratoryStatusCommand(
                laboratoryId,
                request.Status),
            cancellationToken);

        return NoContent();
    }
}
