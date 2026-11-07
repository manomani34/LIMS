using LIMS.Contracts.Inventory.Enums;

namespace LIMS.Contracts.Inventory.Responses;

public sealed record InventoryItemResponse(
    Guid Id,
    Guid LaboratoryId,
    string ItemCode,
    string Name,
    string? Description,
    InventoryItemTypeContract Type,
    InventoryItemStatusContract Status,
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
