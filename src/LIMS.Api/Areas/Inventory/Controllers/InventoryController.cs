using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Inventory.Commands;
using LIMS.Application.Inventory.Queries;
using LIMS.Contracts.Inventory.Mappings;
using LIMS.Contracts.Inventory.Requests;
using LIMS.Contracts.Inventory.Responses;
using LIMS.Domain.Inventory.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Inventory.Controllers;

[Area("Inventory")]
[ApiController]
[Route("api/v1/inventory")]
[Produces("application/json")]
public sealed class InventoryController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public InventoryController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Type))
            return BadRequest("Inventory item type is invalid.");

        var id = await _dispatcher.SendAsync(
            new CreateInventoryItemCommand(
                request.LaboratoryId,
                request.ItemCode,
                request.Name,
                request.Description,
                request.Type.ToDomain(),
                request.CasNumber,
                request.ChemicalFormula,
                request.MolecularWeight,
                request.HazardClassification,
                request.SafetyDataSheetReference,
                request.DefaultUnit,
                request.MinimumStockLevel),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { inventoryItemId = id },
            new { id });
    }

    [HttpGet("{inventoryItemId:guid}")]
    [ProducesResponseType(typeof(InventoryItemDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid inventoryItemId,
        CancellationToken cancellationToken)
    {
        var item = await _dispatcher.QueryAsync(
            new GetInventoryItemByIdQuery(inventoryItemId),
            cancellationToken);

        return item is null
            ? NotFound()
            : Ok(item.ToResponse());
    }

    [HttpGet("by-code/{laboratoryId:guid}/{itemCode}")]
    [ProducesResponseType(typeof(InventoryItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken)
    {
        var item = await _dispatcher.QueryAsync(
            new GetInventoryItemByCodeQuery(laboratoryId, itemCode),
            cancellationToken);

        return item is null
            ? NotFound()
            : Ok(item.ToResponse());
    }

    [HttpGet("by-laboratory/{laboratoryId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<InventoryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLaboratory(
        Guid laboratoryId,
        [FromQuery] InventoryItemStatus? status,
        [FromQuery] InventoryItemType? type,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var items = await _dispatcher.QueryAsync(
            new GetInventoryItemsByLaboratoryQuery(
                laboratoryId,
                status,
                type,
                pageNumber,
                pageSize),
            cancellationToken);

        return Ok(items.Select(x => x.ToResponse()).ToArray());
    }

    [HttpPut("{inventoryItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid inventoryItemId,
        [FromBody] UpdateInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Type))
            return BadRequest("Inventory item type is invalid.");

        var updated = await _dispatcher.SendAsync(
            new UpdateInventoryItemCommand(
                inventoryItemId,
                request.Name,
                request.Description,
                request.Type.ToDomain(),
                request.CasNumber,
                request.ChemicalFormula,
                request.MolecularWeight,
                request.HazardClassification,
                request.SafetyDataSheetReference,
                request.DefaultUnit,
                request.MinimumStockLevel),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/lots")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReceiveStock(
        Guid inventoryItemId,
        [FromBody] ReceiveInventoryStockRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var lotId = await _dispatcher.SendAsync(
            new ReceiveInventoryStockCommand(
                inventoryItemId,
                request.LotNumber,
                request.Supplier,
                request.Manufacturer,
                request.Quantity,
                request.Unit,
                request.MinimumStockLevel,
                request.ManufactureDate,
                request.ExpiryDate,
                request.StorageLocation,
                request.StorageConditions,
                request.CertificateOfAnalysisNumber,
                userId,
                request.ReferenceNumber,
                request.Notes),
            cancellationToken);

        return Created(
            $"/api/v1/inventory/{inventoryItemId}/lots/{lotId}",
            new { lotId });
    }

    [HttpPost("{inventoryItemId:guid}/lots/consume")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsumeStock(
        Guid inventoryItemId,
        [FromBody] ConsumeInventoryStockRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ConsumeInventoryStockCommand(
                inventoryItemId,
                request.LotId,
                request.Quantity,
                GetCurrentUserId(),
                request.ExperimentId,
                request.Notes),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/lots/adjust")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdjustStock(
        Guid inventoryItemId,
        [FromBody] AdjustInventoryStockRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new AdjustInventoryStockCommand(
                inventoryItemId,
                request.LotId,
                request.Quantity,
                request.Increase,
                GetCurrentUserId(),
                request.Reason),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/lots/quarantine")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> QuarantineLot(
        Guid inventoryItemId,
        [FromQuery] Guid lotId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new QuarantineInventoryLotCommand(inventoryItemId, lotId),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/lots/release")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReleaseLot(
        Guid inventoryItemId,
        [FromQuery] Guid lotId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ReleaseInventoryLotCommand(inventoryItemId, lotId),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/lots/dispose")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisposeLot(
        Guid inventoryItemId,
        [FromBody] DisposeInventoryLotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DisposeInventoryLotCommand(
                inventoryItemId,
                request.LotId,
                GetCurrentUserId(),
                request.Reason),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        Guid inventoryItemId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DeactivateInventoryItemCommand(inventoryItemId),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reactivate(
        Guid inventoryItemId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ReactivateInventoryItemCommand(inventoryItemId),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    [HttpPost("{inventoryItemId:guid}/discontinue")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Discontinue(
        Guid inventoryItemId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DiscontinueInventoryItemCommand(inventoryItemId),
            cancellationToken);

        return result ? NoContent() : NotFound();
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(
                   System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
               ?? throw new UnauthorizedAccessException(
                   "Authenticated user ID is unavailable.");
    }
}
