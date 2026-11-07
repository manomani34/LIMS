namespace LIMS.Application.Inventory.DTOs;

public sealed record InventoryItemDetailsDto(
    InventoryItemDto Item,
    IReadOnlyCollection<InventoryLotDto> Lots,
    IReadOnlyCollection<InventoryMovementDto> Movements);
