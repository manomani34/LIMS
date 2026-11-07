using LIMS.Domain.Common;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;

namespace LIMS.Domain.Inventory.Entities;

public sealed class InventoryLot : Entity<Guid>
{
    public string LotNumber { get; private set; } = string.Empty;

    public string? Supplier { get; private set; }

    public string? Manufacturer { get; private set; }

    public decimal InitialQuantity { get; private set; }

    public decimal AvailableQuantity { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public decimal? MinimumStockLevel { get; private set; }

    public DateOnly? ManufactureDate { get; private set; }

    public DateOnly? ExpiryDate { get; private set; }

    public string? StorageLocation { get; private set; }

    public string? StorageConditions { get; private set; }

    public string? CertificateOfAnalysisNumber { get; private set; }

    public InventoryLotStatus Status { get; private set; }

    public DateTime ReceivedAt { get; private set; }

    private InventoryLot()
    {
    }

    internal static InventoryLot Create(
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
        DateTime receivedAt)
    {
        if (string.IsNullOrWhiteSpace(lotNumber))
            throw new InventoryDomainException(
                "Lot number is required.");

        if (quantity <= 0)
            throw new InventoryDomainException(
                "Received quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new InventoryDomainException(
                "Unit is required.");

        if (minimumStockLevel < 0)
            throw new InventoryDomainException(
                "Minimum stock level cannot be negative.");

        if (manufactureDate.HasValue &&
            expiryDate.HasValue &&
            expiryDate.Value < manufactureDate.Value)
        {
            throw new InventoryDomainException(
                "Expiry date cannot be earlier than manufacture date.");
        }

        return new InventoryLot
        {
            Id = Guid.NewGuid(),
            LotNumber = lotNumber.Trim(),
            Supplier = NormalizeOptional(supplier),
            Manufacturer = NormalizeOptional(manufacturer),
            InitialQuantity = quantity,
            AvailableQuantity = quantity,
            Unit = unit.Trim(),
            MinimumStockLevel = minimumStockLevel,
            ManufactureDate = manufactureDate,
            ExpiryDate = expiryDate,
            StorageLocation = NormalizeOptional(storageLocation),
            StorageConditions = NormalizeOptional(storageConditions),
            CertificateOfAnalysisNumber =
                NormalizeOptional(certificateOfAnalysisNumber),
            Status = InventoryLotStatus.Available,
            ReceivedAt = receivedAt
        };
    }

    internal void Consume(decimal quantity)
    {
        EnsureAvailable();

        if (quantity <= 0)
            throw new InventoryDomainException(
                "Consumption quantity must be greater than zero.");

        if (quantity > AvailableQuantity)
            throw new InventoryDomainException(
                "Insufficient available quantity in the selected lot.");

        AvailableQuantity -= quantity;

        if (AvailableQuantity == 0)
            Status = InventoryLotStatus.Depleted;
    }

    internal void IncreaseQuantity(decimal quantity)
    {
        EnsureAvailable();

        if (quantity <= 0)
            throw new InventoryDomainException(
                "Adjustment quantity must be greater than zero.");

        AvailableQuantity += quantity;
    }

    internal void DecreaseQuantity(decimal quantity)
    {
        EnsureAvailable();

        if (quantity <= 0)
            throw new InventoryDomainException(
                "Adjustment quantity must be greater than zero.");

        if (quantity > AvailableQuantity)
            throw new InventoryDomainException(
                "Adjustment cannot reduce available quantity below zero.");

        AvailableQuantity -= quantity;

        if (AvailableQuantity == 0)
            Status = InventoryLotStatus.Depleted;
    }

    internal void Quarantine()
    {
        EnsureAvailable();
        Status = InventoryLotStatus.Quarantined;
    }

    internal void ReleaseFromQuarantine(DateOnly currentDate)
    {
        if (Status != InventoryLotStatus.Quarantined)
            throw new InventoryDomainException(
                "Only quarantined lots can be released.");

        if (ExpiryDate.HasValue &&
            ExpiryDate.Value < currentDate)
        {
            Status = InventoryLotStatus.Expired;
            return;
        }

        Status = AvailableQuantity > 0
            ? InventoryLotStatus.Available
            : InventoryLotStatus.Depleted;
    }

    internal decimal Dispose()
    {
        if (Status is InventoryLotStatus.Disposed
            or InventoryLotStatus.Expired
            or InventoryLotStatus.Depleted)
        {
            throw new InventoryDomainException(
                "This lot cannot be disposed in its current status.");
        }

        var disposedQuantity = AvailableQuantity;

        AvailableQuantity = 0;
        Status = InventoryLotStatus.Disposed;

        return disposedQuantity;
    }

    private void EnsureAvailable()
    {
        if (Status != InventoryLotStatus.Available)
            throw new InventoryDomainException(
                "The selected lot is not available for this operation.");
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
