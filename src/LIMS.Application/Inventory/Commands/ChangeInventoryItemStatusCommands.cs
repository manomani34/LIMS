using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Inventory.Commands;

public sealed record DeactivateInventoryItemCommand(
    Guid InventoryItemId) : ICommand<bool>;

public sealed record ReactivateInventoryItemCommand(
    Guid InventoryItemId) : ICommand<bool>;

public sealed record DiscontinueInventoryItemCommand(
    Guid InventoryItemId) : ICommand<bool>;
