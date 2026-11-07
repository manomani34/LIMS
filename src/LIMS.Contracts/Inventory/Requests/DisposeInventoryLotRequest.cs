namespace LIMS.Contracts.Inventory.Requests;

public sealed record DisposeInventoryLotRequest(
    Guid LotId,
    string Reason);
