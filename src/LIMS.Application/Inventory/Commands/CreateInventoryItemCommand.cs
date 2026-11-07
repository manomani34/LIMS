using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.Commands;

public sealed record CreateInventoryItemCommand(
    Guid LaboratoryId,
    string ItemCode,
    string Name,
    string? Description,
    InventoryItemType Type,
    string? CasNumber,
    string? ChemicalFormula,
    string? MolecularWeight,
    string? HazardClassification,
    string? SafetyDataSheetReference,
    string DefaultUnit,
    decimal MinimumStockLevel) : ICommand<Guid>;
