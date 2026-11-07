namespace LIMS.Contracts.Inventory.Responses;

public sealed record InventoryItemDetailsResponse(
    InventoryItemResponse Item,
    IReadOnlyCollection<InventoryLotResponse> Lots,
    IReadOnlyCollection<InventoryMovementResponse> Movements);
