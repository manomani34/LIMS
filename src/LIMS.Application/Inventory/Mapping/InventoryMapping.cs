using LIMS.Application.Inventory.DTOs;
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Entities;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.Mapping;

public static class InventoryMapping
{
    public static InventoryItemDto ToDto(
        this InventoryItem item,
        DateOnly currentDate)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new InventoryItemDto(
            item.Id,
            item.LaboratoryId,
            item.ItemCode,
            item.Name,
            item.Description,
            item.Type,
            item.Status,
            item.CasNumber,
            item.ChemicalFormula,
            item.MolecularWeight,
            item.HazardClassification,
            item.SafetyDataSheetReference,
            item.DefaultUnit,
            item.MinimumStockLevel,
            item.GetTotalAvailableQuantity(currentDate),
            item.CreatedAt,
            item.UpdatedAt);
    }

    public static InventoryLotDto ToDto(
        this InventoryLot lot,
        DateOnly currentDate)
    {
        ArgumentNullException.ThrowIfNull(lot);

        var effectiveStatus =
            lot.Status == InventoryLotStatus.Available &&
            lot.ExpiryDate.HasValue &&
            lot.ExpiryDate.Value < currentDate
                ? InventoryLotStatus.Expired
                : lot.Status;

        return new InventoryLotDto(
            lot.Id,
            lot.LotNumber,
            lot.Supplier,
            lot.Manufacturer,
            lot.InitialQuantity,
            lot.AvailableQuantity,
            lot.Unit,
            lot.MinimumStockLevel,
            lot.ManufactureDate,
            lot.ExpiryDate,
            lot.StorageLocation,
            lot.StorageConditions,
            lot.CertificateOfAnalysisNumber,
            effectiveStatus,
            lot.ReceivedAt);
    }

    public static InventoryMovementDto ToDto(
        this InventoryMovement movement)
    {
        ArgumentNullException.ThrowIfNull(movement);

        return new InventoryMovementDto(
            movement.Id,
            movement.LotId,
            movement.Type,
            movement.Quantity,
            movement.BalanceAfterMovement,
            movement.Unit,
            movement.PerformedByUserId,
            movement.ExperimentId,
            movement.ReferenceNumber,
            movement.Notes,
            movement.OccurredAt);
    }

    public static InventoryItemDetailsDto ToDetailsDto(
        this InventoryItem item,
        DateOnly currentDate)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new InventoryItemDetailsDto(
            item.ToDto(currentDate),
            item.Lots
                .Select(x => x.ToDto(currentDate))
                .ToArray(),
            item.Movements
                .Select(x => x.ToDto())
                .ToArray());
    }
}
