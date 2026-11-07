namespace LIMS.Contracts.Inventory.Requests;

public sealed record ReceiveInventoryStockRequest(
    string LotNumber,
    string? Supplier,
    string? Manufacturer,
    decimal Quantity,
    string Unit,
    decimal? MinimumStockLevel,
    DateOnly? ManufactureDate,
    DateOnly? ExpiryDate,
    string? StorageLocation,
    string? StorageConditions,
    string? CertificateOfAnalysisNumber,
    string? ReferenceNumber,
    string? Notes);
