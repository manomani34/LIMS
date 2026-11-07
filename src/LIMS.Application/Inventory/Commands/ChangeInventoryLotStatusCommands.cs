using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Inventory.Commands;

public sealed record QuarantineInventoryLotCommand(
    Guid InventoryItemId,
    Guid LotId) : ICommand<bool>;

public sealed record ReleaseInventoryLotCommand(
    Guid InventoryItemId,
    Guid LotId) : ICommand<bool>;

public sealed record DisposeInventoryLotCommand(
    Guid InventoryItemId,
    Guid LotId,
    string PerformedByUserId,
    string Reason) : ICommand<bool>;
