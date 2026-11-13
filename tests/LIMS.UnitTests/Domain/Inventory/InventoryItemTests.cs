
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;

namespace LIMS.UnitTests.Domain.Inventory;

public sealed class InventoryItemTests
{
    private static readonly DateTime CreatedAt =
        new(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);

    private static InventoryItem CreateItem(
        string itemCode = "CHEM-001")
    {
        return InventoryItem.Create(
            Guid.NewGuid(),
            itemCode,
            "Sodium Chloride",
            "Laboratory grade",
            InventoryItemType.Chemical,
            "7647-14-5",
            "NaCl",
            "58.44",
            null,
            null,
            "g",
            100,
            CreatedAt);
    }

    [Fact]
    public void Create_ShouldInitializeInventoryItem()
    {
        var laboratoryId = Guid.NewGuid();

        var item = InventoryItem.Create(
            laboratoryId,
            "CHEM-001",
            "Sodium Chloride",
            "Laboratory grade",
            InventoryItemType.Chemical,
            "7647-14-5",
            "NaCl",
            "58.44",
            null,
            null,
            "g",
            100,
            CreatedAt);

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(laboratoryId, item.LaboratoryId);
        Assert.Equal("CHEM-001", item.ItemCode);
        Assert.Equal("Sodium Chloride", item.Name);
        Assert.Equal("Laboratory grade", item.Description);
        Assert.Equal(InventoryItemType.Chemical, item.Type);
        Assert.Equal(InventoryItemStatus.Active, item.Status);
        Assert.Equal("7647-14-5", item.CasNumber);
        Assert.Equal("NaCl", item.ChemicalFormula);
        Assert.Equal("58.44", item.MolecularWeight);
        Assert.Equal("g", item.DefaultUnit);
        Assert.Equal(100, item.MinimumStockLevel);
        Assert.Equal(CreatedAt, item.CreatedAt);
        Assert.Equal(CreatedAt, item.UpdatedAt);
        Assert.Empty(item.Lots);
        Assert.Empty(item.Movements);
    }

    [Fact]
    public void Create_ShouldNormalizeInputValues()
    {
        var item = InventoryItem.Create(
            Guid.NewGuid(),
            "  CHEM-001  ",
            "  Sodium Chloride  ",
            "  Laboratory grade  ",
            InventoryItemType.Chemical,
            " 7647-14-5 ",
            " NaCl ",
            null,
            null,
            null,
            " g ",
            0,
            CreatedAt);

        Assert.Equal("CHEM-001", item.ItemCode);
        Assert.Equal("Sodium Chloride", item.Name);
        Assert.Equal("Laboratory grade", item.Description);
        Assert.Equal("7647-14-5", item.CasNumber);
        Assert.Equal("NaCl", item.ChemicalFormula);
        Assert.Equal("g", item.DefaultUnit);
    }

    [Fact]
    public void Create_ShouldNormalizeEmptyOptionalValuesToNull()
    {
        var item = InventoryItem.Create(
            Guid.NewGuid(),
            "CHEM-001",
            "Sodium Chloride",
            " ",
            InventoryItemType.Chemical,
            null,
            " ",
            null,
            null,
            " ",
            "g",
            0,
            CreatedAt);

        Assert.Null(item.Description);
        Assert.Null(item.CasNumber);
        Assert.Null(item.ChemicalFormula);
        Assert.Null(item.MolecularWeight);
        Assert.Null(item.HazardClassification);
        Assert.Null(item.SafetyDataSheetReference);
    }

    [Fact]
    public void Create_ShouldRejectEmptyLaboratoryId()
    {
        Assert.Throws<InventoryDomainException>(() =>
            InventoryItem.Create(
                Guid.Empty,
                "CHEM-001",
                "Sodium Chloride",
                null,
                InventoryItemType.Chemical,
                null,
                null,
                null,
                null,
                null,
                "g",
                0,
                CreatedAt));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("CHEM 001")]
    [InlineData("CHEM.001")]
    [InlineData("CHEM/001")]
    [InlineData("شیمی-001")]
    public void Create_ShouldRejectInvalidItemCode(string itemCode)
    {
        Assert.Throws<InventoryDomainException>(() =>
            InventoryItem.Create(
                Guid.NewGuid(),
                itemCode,
                "Sodium Chloride",
                null,
                InventoryItemType.Chemical,
                null,
                null,
                null,
                null,
                null,
                "g",
                0,
                CreatedAt));
    }

    [Fact]
    public void Create_ShouldAcceptValidItemCodeCharacters()
    {
        var item = CreateItem("CHEM_001-NaCl");

        Assert.Equal("CHEM_001-NaCl", item.ItemCode);
    }

    [Fact]
    public void Create_ShouldRejectNegativeMinimumStockLevel()
    {
        Assert.Throws<InventoryDomainException>(() =>
            InventoryItem.Create(
                Guid.NewGuid(),
                "CHEM-001",
                "Sodium Chloride",
                null,
                InventoryItemType.Chemical,
                null,
                null,
                null,
                null,
                null,
                "g",
                -1,
                CreatedAt));
    }

    [Fact]
    public void Create_ShouldRejectInvalidItemType()
    {
        Assert.Throws<InventoryDomainException>(() =>
            InventoryItem.Create(
                Guid.NewGuid(),
                "CHEM-001",
                "Sodium Chloride",
                null,
                (InventoryItemType)999,
                null,
                null,
                null,
                null,
                null,
                "g",
                0,
                CreatedAt));
    }

    [Fact]
    public void UpdateInformation_ShouldUpdateItemInformation()
    {
        var item = CreateItem();
        var updatedAt = CreatedAt.AddHours(1);

        item.UpdateInformation(
            "Updated Sodium Chloride",
            "Updated description",
            InventoryItemType.Reagent,
            "7647-14-5",
            "NaCl",
            "58.44",
            "Non-hazardous",
            "SDS-001",
            "g",
            200,
            updatedAt);

        Assert.Equal("Updated Sodium Chloride", item.Name);
        Assert.Equal("Updated description", item.Description);
        Assert.Equal(InventoryItemType.Reagent, item.Type);
        Assert.Equal("Non-hazardous", item.HazardClassification);
        Assert.Equal("SDS-001", item.SafetyDataSheetReference);
        Assert.Equal(200, item.MinimumStockLevel);
        Assert.Equal(updatedAt, item.UpdatedAt);
    }

    [Fact]
    public void UpdateInformation_ShouldRejectNegativeMinimumStockLevel()
    {
        var item = CreateItem();

        Assert.Throws<InventoryDomainException>(() =>
            item.UpdateInformation(
                "Sodium Chloride",
                null,
                InventoryItemType.Chemical,
                null,
                null,
                null,
                null,
                null,
                "g",
                -1,
                CreatedAt.AddHours(1)));
    }

    [Fact]
    public void UpdateInformation_ShouldRejectUnitChangeWhenLotsExist()
    {
        var item = CreateItem();

        item.ReceiveStock(
            "LOT-001",
            null,
            null,
            100,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        Assert.Throws<InventoryDomainException>(() =>
            item.UpdateInformation(
                "Sodium Chloride",
                null,
                InventoryItemType.Chemical,
                null,
                null,
                null,
                null,
                null,
                "kg",
                100,
                CreatedAt.AddHours(1)));
    }

    [Fact]
    public void UpdateInformation_ShouldAllowSameUnitWhenLotsExist()
    {
        var item = CreateItem();

        item.ReceiveStock(
            "LOT-001",
            null,
            null,
            100,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        item.UpdateInformation(
            "Updated Sodium Chloride",
            null,
            InventoryItemType.Chemical,
            null,
            null,
            null,
            null,
            null,
            "G",
            100,
            CreatedAt.AddHours(1));

        Assert.Equal("G", item.DefaultUnit);
    }

    [Fact]
    public void ReceiveStock_ShouldCreateLotAndReceiptMovement()
    {
        var item = CreateItem();

        var lotId = item.ReceiveStock(
            "LOT-001",
            "Supplier A",
            "Manufacturer A",
            100,
            "g",
            null,
            null,
            null,
            "Shelf A",
            "Room temperature",
            "COA-001",
            "user-001",
            "REF-001",
            "Initial receipt",
            CreatedAt);

        var lot = Assert.Single(item.Lots);
        var movement = Assert.Single(item.Movements);

        Assert.Equal(lotId, lot.Id);
        Assert.Equal("LOT-001", lot.LotNumber);
        Assert.Equal(100, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);

        Assert.Equal(lotId, movement.LotId);
        Assert.Equal(InventoryMovementType.Receipt, movement.Type);
        Assert.Equal(100, movement.Quantity);
        Assert.Equal(100, movement.BalanceAfterMovement);
        Assert.Equal("user-001", movement.PerformedByUserId);
        Assert.Equal("REF-001", movement.ReferenceNumber);
        Assert.Equal("Initial receipt", movement.Notes);
    }

    [Fact]
    public void ReceiveStock_ShouldRejectWhenItemIsInactive()
    {
        var item = CreateItem();
        item.Deactivate(CreatedAt.AddMinutes(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.ReceiveStock(
                "LOT-001",
                null,
                null,
                100,
                "g",
                null,
                null,
                null,
                null,
                null,
                null,
                "user-001",
                null,
                null,
                CreatedAt.AddMinutes(2)));
    }

    [Fact]
    public void ReceiveStock_ShouldRejectWhenUnitDoesNotMatchDefaultUnit()
    {
        var item = CreateItem();

        Assert.Throws<InventoryDomainException>(() =>
            item.ReceiveStock(
                "LOT-001",
                null,
                null,
                100,
                "kg",
                null,
                null,
                null,
                null,
                null,
                null,
                "user-001",
                null,
                null,
                CreatedAt));
    }

    [Fact]
    public void ReceiveStock_ShouldRejectDuplicateLotNumber()
    {
        var item = CreateItem();

        item.ReceiveStock(
            "LOT-001",
            null,
            null,
            100,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        Assert.Throws<InventoryDomainException>(() =>
            item.ReceiveStock(
                "lot-001",
                null,
                null,
                50,
                "g",
                null,
                null,
                null,
                null,
                null,
                null,
                "user-001",
                null,
                null,
                CreatedAt.AddHours(1)));
    }

    [Fact]
    public void ConsumeStock_ShouldAllowConsumptionForDiscontinuedItem()
    {
        var item = CreateItem();

        var lotId = item.ReceiveStock(
            "LOT-001",
            null,
            null,
            100,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        item.Discontinue(CreatedAt.AddMinutes(1));

        item.ConsumeStock(
            lotId,
            30,
            "user-002",
            Guid.NewGuid(),
            "Experiment consumption",
            DateOnly.FromDateTime(CreatedAt),
            CreatedAt.AddHours(1));

        Assert.Equal(70, Assert.Single(item.Lots).AvailableQuantity);
        Assert.Equal(InventoryItemStatus.Discontinued, item.Status);
        Assert.Equal(2, item.Movements.Count);
    }

    [Fact]
    public void ConsumeStock_ShouldRejectWhenItemIsInactive()
    {
        var item = CreateItem();

        var lotId = item.ReceiveStock(
            "LOT-001",
            null,
            null,
            100,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        item.Deactivate(CreatedAt.AddMinutes(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.ConsumeStock(
                lotId,
                10,
                "user-002",
                null,
                null,
                DateOnly.FromDateTime(CreatedAt),
                CreatedAt.AddHours(1)));
    }

    [Fact]
    public void GetTotalAvailableQuantity_ShouldReturnOnlyAvailableNonExpiredLots()
    {
        var item = CreateItem();
        var currentDate = new DateOnly(2026, 11, 10);

        item.ReceiveStock(
            "LOT-AVAILABLE",
            null,
            null,
            100,
            "g",
            null,
            null,
            currentDate.AddDays(10),
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        item.ReceiveStock(
            "LOT-EXPIRED",
            null,
            null,
            50,
            "g",
            null,
            null,
            currentDate.AddDays(-1),
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        Assert.Equal(100, item.GetTotalAvailableQuantity(currentDate));
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusToInactive()
    {
        var item = CreateItem();

        item.Deactivate(CreatedAt.AddHours(1));

        Assert.Equal(InventoryItemStatus.Inactive, item.Status);
        Assert.Equal(CreatedAt.AddHours(1), item.UpdatedAt);
    }

    [Fact]
    public void Reactivate_ShouldChangeInactiveItemToActive()
    {
        var item = CreateItem();
        item.Deactivate(CreatedAt.AddHours(1));

        item.Reactivate(CreatedAt.AddHours(2));

        Assert.Equal(InventoryItemStatus.Active, item.Status);
    }

    [Fact]
    public void Discontinue_ShouldChangeStatusToDiscontinued()
    {
        var item = CreateItem();

        item.Discontinue(CreatedAt.AddHours(1));

        Assert.Equal(InventoryItemStatus.Discontinued, item.Status);
    }

    [Fact]
    public void DiscontinuedItem_ShouldRejectInformationUpdate()
    {
        var item = CreateItem();
        item.Discontinue(CreatedAt.AddHours(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.UpdateInformation(
                "Updated name",
                null,
                InventoryItemType.Chemical,
                null,
                null,
                null,
                null,
                null,
                "g",
                100,
                CreatedAt.AddHours(2)));
    }

    [Fact]
    public void DiscontinuedItem_ShouldRejectStockAdjustment()
    {
        var item = CreateItem();

        var lotId = item.ReceiveStock(
            "LOT-001",
            null,
            null,
            100,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            null,
            null,
            CreatedAt);

        item.Discontinue(CreatedAt.AddMinutes(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.AdjustStock(
                lotId,
                10,
                true,
                "user-001",
                "Adjustment",
                CreatedAt.AddHours(1)));
    }

    [Fact]
    public void DiscontinuedItem_ShouldRejectNewStockReceipt()
    {
        var item = CreateItem();
        item.Discontinue(CreatedAt.AddMinutes(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.ReceiveStock(
                "LOT-002",
                null,
                null,
                100,
                "g",
                null,
                null,
                null,
                null,
                null,
                null,
                "user-001",
                null,
                null,
                CreatedAt.AddHours(1)));
    }
}
