
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Laboratories;

namespace LIMS.UnitTests.Application.Inventory.Fakes;

public static class InventoryTestData
{
    public static readonly DateTime CreatedAt =
        new(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);

    public static readonly DateTimeOffset CurrentUtc =
        new(2026, 11, 13, 12, 0, 0, TimeSpan.Zero);

    public static InventoryItem CreateItem(
        Guid? laboratoryId = null,
        string itemCode = "CHEM-001",
        string name = "Sodium Chloride",
        InventoryItemType type = InventoryItemType.Chemical)
    {
        return InventoryItem.Create(
            laboratoryId ?? Guid.NewGuid(),
            itemCode,
            name,
            "Laboratory grade",
            type,
            "7647-14-5",
            "NaCl",
            "58.44",
            null,
            null,
            "g",
            100,
            CreatedAt);
    }

    public static Laboratory CreateLaboratory()
    {
        return Laboratory.Create(
            "Chemistry Laboratory",
            $"LAB-{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
            "Inventory test laboratory",
            CurrentUtc);
    }

    public static Guid ReceiveLot(
        InventoryItem item,
        string lotNumber = "LOT-001",
        decimal quantity = 100,
        DateOnly? expiryDate = null)
    {
        return item.ReceiveStock(
            lotNumber,
            "Supplier A",
            "Manufacturer A",
            quantity,
            "g",
            null,
            null,
            expiryDate,
            "Shelf A",
            "Room temperature",
            "COA-001",
            "user-001",
            "REF-001",
            "Test receipt",
            CreatedAt);
    }
}
