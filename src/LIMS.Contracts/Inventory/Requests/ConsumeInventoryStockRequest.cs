namespace LIMS.Contracts.Inventory.Requests;

public sealed record ConsumeInventoryStockRequest(
    Guid LotId,
    decimal Quantity,
    Guid? ExperimentId,
    string? Notes);
