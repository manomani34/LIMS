using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Inventory.Commands;

public sealed record ReceiveInventoryStockCommand(
    Guid InventoryItemId,
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
    string PerformedByUserId,
    string? ReferenceNumber,
    string? Notes) : ICommand<Guid>;
