using LIMS.Contracts.Inventory.Enums;

namespace LIMS.Contracts.Inventory.Responses;

public sealed record InventoryLotResponse(
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
    InventoryLotStatusContract Status,
    DateTime ReceivedAt);
