using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.DTOs;

public sealed record InventoryItemDto(
    Guid Id,
    Guid LaboratoryId,
    string ItemCode,
    string Name,
    string? Description,
    InventoryItemType Type,
    InventoryItemStatus Status,
    string? CasNumber,
    string? ChemicalFormula,
    string? MolecularWeight,
    string? HazardClassification,
    string? SafetyDataSheetReference,
    string DefaultUnit,
    decimal MinimumStockLevel,
    decimal TotalAvailableQuantity,
    DateTime CreatedAt,
    DateTime UpdatedAt);
