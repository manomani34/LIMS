using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Inventory.Commands;

public sealed record AdjustInventoryStockCommand(
    Guid InventoryItemId,
    Guid LotId,
    decimal Quantity,
    bool Increase,
    string PerformedByUserId,
    string Reason) : ICommand<bool>;
