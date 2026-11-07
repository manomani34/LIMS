using LIMS.Contracts.Inventory.Enums;

namespace LIMS.Contracts.Inventory.Responses;

public sealed record InventoryMovementResponse(
    Guid Id,
    Guid LotId,
    InventoryMovementTypeContract Type,
    decimal Quantity,
    decimal BalanceAfterMovement,
    string Unit,
    string PerformedByUserId,
    Guid? ExperimentId,
    string? ReferenceNumber,
    string? Notes,
    DateTime OccurredAt);
