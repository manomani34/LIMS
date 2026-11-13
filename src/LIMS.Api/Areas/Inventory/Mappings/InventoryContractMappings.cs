using LIMS.Contracts.Inventory.Enums;
using LIMS.Contracts.Inventory.Responses;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.Contracts.Inventory.Mappings;

public static class InventoryContractMappings
{
    public static InventoryItemStatus ToDomain(
    this InventoryItemStatusContract value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (InventoryItemStatus)(int)value;
    }

    public static InventoryItemType ToDomain(
        this InventoryItemTypeContract value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (InventoryItemType)(int)value;
    }

    public static InventoryItemTypeContract ToContract(
        this InventoryItemType value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (InventoryItemTypeContract)(int)value;
    }

    public static InventoryItemStatusContract ToContract(
        this InventoryItemStatus value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (InventoryItemStatusContract)(int)value;
    }

    public static InventoryLotStatusContract ToContract(
        this InventoryLotStatus value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (InventoryLotStatusContract)(int)value;
    }

    public static InventoryMovementTypeContract ToContract(
        this InventoryMovementType value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return (InventoryMovementTypeContract)(int)value;
    }

    public static InventoryItemResponse ToResponse(
        this LIMS.Application.Inventory.DTOs.InventoryItemDto dto)
    {
        return new InventoryItemResponse(
            dto.Id,
            dto.LaboratoryId,
            dto.ItemCode,
            dto.Name,
            dto.Description,
            dto.Type.ToContract(),
            dto.Status.ToContract(),
            dto.CasNumber,
            dto.ChemicalFormula,
            dto.MolecularWeight,
            dto.HazardClassification,
            dto.SafetyDataSheetReference,
            dto.DefaultUnit,
            dto.MinimumStockLevel,
            dto.TotalAvailableQuantity,
            dto.CreatedAt,
            dto.UpdatedAt);
    }

    public static InventoryLotResponse ToResponse(
        this LIMS.Application.Inventory.DTOs.InventoryLotDto dto)
    {
        return new InventoryLotResponse(
            dto.Id,
            dto.LotNumber,
            dto.Supplier,
            dto.Manufacturer,
            dto.InitialQuantity,
            dto.AvailableQuantity,
            dto.Unit,
            dto.MinimumStockLevel,
            dto.ManufactureDate,
            dto.ExpiryDate,
            dto.StorageLocation,
            dto.StorageConditions,
            dto.CertificateOfAnalysisNumber,
            dto.Status.ToContract(),
            dto.ReceivedAt);
    }

    public static InventoryMovementResponse ToResponse(
        this LIMS.Application.Inventory.DTOs.InventoryMovementDto dto)
    {
        return new InventoryMovementResponse(
            dto.Id,
            dto.LotId,
            dto.Type.ToContract(),
            dto.Quantity,
            dto.BalanceAfterMovement,
            dto.Unit,
            dto.PerformedByUserId,
            dto.ExperimentId,
            dto.ReferenceNumber,
            dto.Notes,
            dto.OccurredAt);
    }

    public static InventoryItemDetailsResponse ToResponse(
        this LIMS.Application.Inventory.DTOs.InventoryItemDetailsDto dto)
    {
        return new InventoryItemDetailsResponse(
            dto.Item.ToResponse(),
            dto.Lots.Select(x => x.ToResponse()).ToArray(),
            dto.Movements.Select(x => x.ToResponse()).ToArray());
    }
}
