using LIMS.Application.Inventory.DTOs;
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Entities;

namespace LIMS.Application.Inventory.Mapping;

public static class InventoryMapping
{
    public static InventoryItemDto ToDto(this InventoryItem item)
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
            item.TotalAvailableQuantity,
            item.CreatedAt,
            item.UpdatedAt);
    }

    public static InventoryLotDto ToDto(this InventoryLot lot)
    {
        ArgumentNullException.ThrowIfNull(lot);

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
            lot.Status,
            lot.ReceivedAt);
    }

    public static InventoryMovementDto ToDto(this InventoryMovement movement)
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
        this InventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new InventoryItemDetailsDto(
            item.ToDto(),
            item.Lots.Select(x => x.ToDto()).ToArray(),
            item.Movements.Select(x => x.ToDto()).ToArray());
    }
}
