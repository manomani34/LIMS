using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Inventory.Commands;

public sealed record ConsumeInventoryStockCommand(
    Guid InventoryItemId,
    Guid LotId,
    decimal Quantity,
    string ConsumedByUserId,
    Guid? ExperimentId,
    string? Notes) : ICommand<bool>;
