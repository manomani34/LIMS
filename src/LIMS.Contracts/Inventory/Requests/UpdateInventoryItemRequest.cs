using LIMS.Contracts.Inventory.Enums;

namespace LIMS.Contracts.Inventory.Requests;

public sealed record UpdateInventoryItemRequest(
    string Name,
    string? Description,
    InventoryItemTypeContract Type,
    string? CasNumber,
    string? ChemicalFormula,
    string? MolecularWeight,
    string? HazardClassification,
    string? SafetyDataSheetReference,
    string DefaultUnit,
    decimal MinimumStockLevel);
