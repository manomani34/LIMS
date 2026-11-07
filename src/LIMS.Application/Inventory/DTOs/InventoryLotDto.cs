using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.DTOs;

public sealed record InventoryLotDto(
    Guid Id,
    string LotNumber,
    string? Supplier,
    string? Manufacturer,
    decimal InitialQuantity,
    decimal AvailableQuantity,
    string Unit,
    decimal? MinimumStockLevel,
    DateOnly? ManufactureDate,
    DateOnly? ExpiryDate,
    string? StorageLocation,
    string? StorageConditions,
    string? CertificateOfAnalysisNumber,
    InventoryLotStatus Status,
    DateTime ReceivedAt);
