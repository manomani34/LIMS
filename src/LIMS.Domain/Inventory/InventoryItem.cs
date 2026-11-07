using LIMS.Domain.Common;
using LIMS.Domain.Inventory.Entities;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;

namespace LIMS.Domain.Inventory;

public sealed class InventoryItem : AggregateRoot<Guid>
{
    private readonly List<InventoryLot> _lots = [];
    private readonly List<InventoryMovement> _movements = [];

    public Guid LaboratoryId { get; private set; }

    public string ItemCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public InventoryItemType Type { get; private set; }

    public InventoryItemStatus Status { get; private set; }

    public string? CasNumber { get; private set; }

    public string? ChemicalFormula { get; private set; }

    public string? MolecularWeight { get; private set; }

    public string? HazardClassification { get; private set; }

    public string? SafetyDataSheetReference { get; private set; }

    public string DefaultUnit { get; private set; } = string.Empty;

    public decimal MinimumStockLevel { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public decimal TotalAvailableQuantity =>
        _lots
            .Where(x => x.Status == InventoryLotStatus.Available)
            .Sum(x => x.AvailableQuantity);

    public IReadOnlyCollection<InventoryLot> Lots =>
        _lots.AsReadOnly();

    public IReadOnlyCollection<InventoryMovement> Movements =>
        _movements.AsReadOnly();

    private InventoryItem()
    {
    }

    public static InventoryItem Create(
        Guid laboratoryId,
        string itemCode,
        string name,
        string? description,
        InventoryItemType type,
        string? casNumber,
        string? chemicalFormula,
        string? molecularWeight,
        string? hazardClassification,
        string? safetyDataSheetReference,
        string defaultUnit,
        decimal minimumStockLevel,
        DateTime createdAt)
    {
        if (laboratoryId == Guid.Empty)
            throw new InventoryDomainException(
                "Laboratory identifier is required.");

        if (string.IsNullOrWhiteSpace(itemCode))
            throw new InventoryDomainException(
                "Inventory item code is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new InventoryDomainException(
                "Inventory item name is required.");

        if (string.IsNullOrWhiteSpace(defaultUnit))
            throw new InventoryDomainException(
                "Default unit is required.");

        if (!Enum.IsDefined(type))
            throw new InventoryDomainException(
                "Invalid inventory item type.");

        if (minimumStockLevel < 0)
            throw new InventoryDomainException(
                "Minimum stock level cannot be negative.");

        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            LaboratoryId = laboratoryId,
            ItemCode = itemCode.Trim(),
            Name = name.Trim(),
            Description = NormalizeOptional(description),
            Type = type,
            Status = InventoryItemStatus.Active,
            CasNumber = NormalizeOptional(casNumber),
            ChemicalFormula = NormalizeOptional(chemicalFormula),
            MolecularWeight = NormalizeOptional(molecularWeight),
            HazardClassification = NormalizeOptional(hazardClassification),
            SafetyDataSheetReference =
                NormalizeOptional(safetyDataSheetReference),
            DefaultUnit = defaultUnit.Trim(),
            MinimumStockLevel = minimumStockLevel,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }

    public void UpdateInformation(
        string name,
        string? description,
        InventoryItemType type,
        string? casNumber,
        string? chemicalFormula,
        string? molecularWeight,
        string? hazardClassification,
        string? safetyDataSheetReference,
        string defaultUnit,
        decimal minimumStockLevel,
        DateTime updatedAt)
    {
        EnsureNotDiscontinued();

        if (string.IsNullOrWhiteSpace(name))
            throw new InventoryDomainException(
                "Inventory item name is required.");

        if (string.IsNullOrWhiteSpace(defaultUnit))
            throw new InventoryDomainException(
                "Default unit is required.");

        if (!Enum.IsDefined(type))
            throw new InventoryDomainException(
                "Invalid inventory item type.");

        if (minimumStockLevel < 0)
            throw new InventoryDomainException(
                "Minimum stock level cannot be negative.");

        if (_lots.Any(x =>
                !string.Equals(
                    x.Unit,
                    defaultUnit.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new InventoryDomainException(
                "Default unit cannot be changed while lots use a different unit.");
        }

        Name = name.Trim();
        Description = NormalizeOptional(description);
        Type = type;
        CasNumber = NormalizeOptional(casNumber);
        ChemicalFormula = NormalizeOptional(chemicalFormula);
        MolecularWeight = NormalizeOptional(molecularWeight);
        HazardClassification = NormalizeOptional(hazardClassification);
        SafetyDataSheetReference =
            NormalizeOptional(safetyDataSheetReference);
        DefaultUnit = defaultUnit.Trim();
        MinimumStockLevel = minimumStockLevel;
        UpdatedAt = updatedAt;
    }

    public Guid ReceiveStock(
        string lotNumber,
        string? supplier,
        string? manufacturer,
        decimal quantity,
        string unit,
        decimal? minimumStockLevel,
        DateOnly? manufactureDate,
        DateOnly? expiryDate,
        string? storageLocation,
        string? storageConditions,
        string? certificateOfAnalysisNumber,
        string performedByUserId,
        string? referenceNumber,
        string? notes,
        DateTime occurredAt)
    {
        EnsureActive();

        if (!string.Equals(
                DefaultUnit,
                unit?.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InventoryDomainException(
                $"Lot unit must match the inventory item's default unit '{DefaultUnit}'.");
        }

        if (_lots.Any(x =>
                string.Equals(
                    x.LotNumber,
                    lotNumber?.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new InventoryDomainException(
                $"Lot number '{lotNumber}' already exists for this inventory item.");
        }

        var lot = InventoryLot.Create(
            lotNumber,
            supplier,
            manufacturer,
            quantity,
            unit,
            minimumStockLevel,
            manufactureDate,
            expiryDate,
            storageLocation,
            storageConditions,
            certificateOfAnalysisNumber,
            occurredAt);

        _lots.Add(lot);

        AddMovement(
            lot,
            InventoryMovementType.Receipt,
            quantity,
            performedByUserId,
            null,
            referenceNumber,
            notes,
            occurredAt);

        UpdatedAt = occurredAt;

        return lot.Id;
    }

    public void ConsumeStock(
        Guid lotId,
        decimal quantity,
        string consumedByUserId,
        Guid? experimentId,
        string? notes,
        DateTime occurredAt)
    {
        EnsureActive();

        var lot = GetLot(lotId);

        lot.Consume(quantity);

        AddMovement(
            lot,
            InventoryMovementType.Consumption,
            quantity,
            consumedByUserId,
            experimentId,
            null,
            notes,
            occurredAt);

        UpdatedAt = occurredAt;
    }

    public void AdjustStock(
        Guid lotId,
        decimal quantity,
        bool increase,
        string performedByUserId,
        string reason,
        DateTime occurredAt)
    {
        EnsureActive();

        if (string.IsNullOrWhiteSpace(reason))
            throw new InventoryDomainException(
                "Adjustment reason is required.");

        var lot = GetLot(lotId);

        if (increase)
        {
            lot.IncreaseQuantity(quantity);

            AddMovement(
                lot,
                InventoryMovementType.AdjustmentIncrease,
                quantity,
                performedByUserId,
                null,
                null,
                reason,
                occurredAt);
        }
        else
        {
            lot.DecreaseQuantity(quantity);

            AddMovement(
                lot,
                InventoryMovementType.AdjustmentDecrease,
                quantity,
                performedByUserId,
                null,
                null,
                reason,
                occurredAt);
        }

        UpdatedAt = occurredAt;
    }

    public void QuarantineLot(
        Guid lotId,
        DateTime occurredAt)
    {
        var lot = GetLot(lotId);

        lot.Quarantine();

        UpdatedAt = occurredAt;
    }

    public void ReleaseLotFromQuarantine(
        Guid lotId,
        DateOnly currentDate,
        DateTime occurredAt)
    {
        var lot = GetLot(lotId);

        lot.ReleaseFromQuarantine(currentDate);

        UpdatedAt = occurredAt;
    }

    public void DisposeLot(
        Guid lotId,
        string performedByUserId,
        string reason,
        DateTime occurredAt)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InventoryDomainException(
                "Disposal reason is required.");

        var lot = GetLot(lotId);

        var disposedQuantity = lot.Dispose();

        if (disposedQuantity > 0)
        {
            AddMovement(
                lot,
                InventoryMovementType.Disposal,
                disposedQuantity,
                performedByUserId,
                null,
                null,
                reason,
                occurredAt);
        }

        UpdatedAt = occurredAt;
    }

    public void Deactivate(DateTime occurredAt)
    {
        if (Status != InventoryItemStatus.Active)
            throw new InventoryDomainException(
                "Only active inventory items can be deactivated.");

        Status = InventoryItemStatus.Inactive;
        UpdatedAt = occurredAt;
    }

    public void Reactivate(DateTime occurredAt)
    {
        if (Status != InventoryItemStatus.Inactive)
            throw new InventoryDomainException(
                "Only inactive inventory items can be reactivated.");

        Status = InventoryItemStatus.Active;
        UpdatedAt = occurredAt;
    }

    public void Discontinue(DateTime occurredAt)
    {
        if (Status == InventoryItemStatus.Discontinued)
            throw new InventoryDomainException(
                "Inventory item is already discontinued.");

        Status = InventoryItemStatus.Discontinued;
        UpdatedAt = occurredAt;
    }

    private InventoryLot GetLot(Guid lotId)
    {
        if (lotId == Guid.Empty)
            throw new InventoryDomainException(
                "Lot identifier is required.");

        return _lots.FirstOrDefault(x => x.Id == lotId)
            ?? throw new InventoryDomainException(
                $"Inventory lot '{lotId}' was not found.");
    }

    private void AddMovement(
        InventoryLot lot,
        InventoryMovementType type,
        decimal quantity,
        string performedByUserId,
        Guid? experimentId,
        string? referenceNumber,
        string? notes,
        DateTime occurredAt)
    {
        var movement = InventoryMovement.Create(
            lot.Id,
            type,
            quantity,
            lot.AvailableQuantity,
            lot.Unit,
            performedByUserId,
            experimentId,
            referenceNumber,
            notes,
            occurredAt);

        _movements.Add(movement);
    }

    private void EnsureActive()
    {
        if (Status != InventoryItemStatus.Active)
            throw new InventoryDomainException(
                "Inventory operations are allowed only for active items.");
    }

    private void EnsureNotDiscontinued()
    {
        if (Status == InventoryItemStatus.Discontinued)
            throw new InventoryDomainException(
                "Discontinued inventory items cannot be modified.");
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
