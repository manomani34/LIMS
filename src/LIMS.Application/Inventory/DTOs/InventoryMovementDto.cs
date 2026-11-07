using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.DTOs;

public sealed record InventoryMovementDto(
    Guid Id,
    Guid LotId,
    InventoryMovementType Type,
    decimal Quantity,
    decimal BalanceAfterMovement,
    string Unit,
    string PerformedByUserId,
    Guid? ExperimentId,
    string? ReferenceNumber,
    string? Notes,
    DateTime OccurredAt);
