namespace LIMS.Contracts.Inventory.Requests;

public sealed record AdjustInventoryStockRequest(
    Guid LotId,
    decimal Quantity,
    bool Increase,
    string Reason);
