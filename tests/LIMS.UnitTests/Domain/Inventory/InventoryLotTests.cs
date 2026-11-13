
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;

namespace LIMS.UnitTests.Domain.Inventory;

public sealed class InventoryLotTests
{
    private static readonly DateTime ReceivedAt =
        new(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);

    private static readonly DateOnly CurrentDate =
        new(2026, 11, 10);

    private static InventoryItem CreateItem()
    {
        return InventoryItem.Create(
            Guid.NewGuid(),
            "CHEM-001",
            "Sodium Chloride",
            null,
            InventoryItemType.Chemical,
            null,
            "NaCl",
            null,
            null,
            null,
            "g",
            0,
            ReceivedAt);
    }

    private static Guid ReceiveLot(
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
            null,
            null,
            "user-001",
            null,
            null,
            ReceivedAt);
    }

    [Fact]
    public void ReceiveStock_ShouldCreateAvailableLot()
    {
        var item = CreateItem();

        var lotId = ReceiveLot(item);

        var lot = Assert.Single(item.Lots);

        Assert.Equal(lotId, lot.Id);
        Assert.Equal("LOT-001", lot.LotNumber);
        Assert.Equal(100, lot.InitialQuantity);
        Assert.Equal(100, lot.AvailableQuantity);
        Assert.Equal("g", lot.Unit);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);
        Assert.Equal(ReceivedAt, lot.ReceivedAt);
    }

    [Fact]
    public void ReceiveStock_ShouldCreateExpiredLotWhenExpiryDateIsInPast()
    {
        var item = CreateItem();

        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(lotId, lot.Id);
        Assert.Equal(InventoryLotStatus.Expired, lot.Status);
        Assert.Equal(100, lot.AvailableQuantity);
    }

    [Fact]
    public void ReceiveStock_ShouldAllowLotExpiringOnCurrentDate()
    {
        var item = CreateItem();

        ReceiveLot(item, expiryDate: CurrentDate);

        var lot = Assert.Single(item.Lots);

        Assert.Equal(InventoryLotStatus.Available, lot.Status);
    }

    [Fact]
    public void ReceiveStock_ShouldRejectZeroQuantity()
    {
        var item = CreateItem();

        Assert.Throws<InventoryDomainException>(() =>
            ReceiveLot(item, quantity: 0));
    }

    [Fact]
    public void ReceiveStock_ShouldRejectNegativeQuantity()
    {
        var item = CreateItem();

        Assert.Throws<InventoryDomainException>(() =>
            ReceiveLot(item, quantity: -10));
    }

    [Fact]
    public void ReceiveStock_ShouldRejectInvalidDateRange()
    {
        var item = CreateItem();

        Assert.Throws<InventoryDomainException>(() =>
            item.ReceiveStock(
                "LOT-001",
                null,
                null,
                100,
                "g",
                null,
                CurrentDate,
                CurrentDate.AddDays(-1),
                null,
                null,
                null,
                "user-001",
                null,
                null,
                ReceivedAt));
    }

    [Fact]
    public void ConsumeStock_ShouldDecreaseAvailableQuantity()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.ConsumeStock(
            lotId,
            30,
            "user-002",
            null,
            null,
            CurrentDate,
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(70, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);
    }

    [Fact]
    public void ConsumeStock_ShouldSetStatusToDepletedWhenQuantityReachesZero()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.ConsumeStock(
            lotId,
            100,
            "user-002",
            null,
            null,
            CurrentDate,
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(0, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Depleted, lot.Status);
    }

    [Fact]
    public void ConsumeStock_ShouldRejectExpiredLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        Assert.Throws<InventoryDomainException>(() =>
            item.ConsumeStock(
                lotId,
                10,
                "user-002",
                null,
                null,
                CurrentDate,
                ReceivedAt.AddHours(1)));
    }

    [Fact]
    public void ConsumeStock_ShouldRejectQuantityGreaterThanAvailable()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        Assert.Throws<InventoryDomainException>(() =>
            item.ConsumeStock(
                lotId,
                101,
                "user-002",
                null,
                null,
                CurrentDate,
                ReceivedAt.AddHours(1)));
    }

    [Fact]
    public void AdjustStock_ShouldIncreaseAvailableLotQuantity()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.AdjustStock(
            lotId,
            50,
            true,
            "user-001",
            "Stock correction",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(150, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);
    }

    [Fact]
    public void AdjustStock_ShouldDecreaseAvailableLotQuantity()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.AdjustStock(
            lotId,
            30,
            false,
            "user-001",
            "Stock correction",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(70, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);
    }

    [Fact]
    public void AdjustStock_ShouldAllowIncreasingExpiredLotWithoutChangingStatus()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        item.AdjustStock(
            lotId,
            20,
            true,
            "user-001",
            "Expired stock correction",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(120, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Expired, lot.Status);
    }

    [Fact]
    public void AdjustStock_ShouldAllowDecreasingExpiredLotWithoutChangingStatus()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        item.AdjustStock(
            lotId,
            20,
            false,
            "user-001",
            "Expired stock correction",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(80, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Expired, lot.Status);
    }

    [Fact]
    public void AdjustStock_ShouldSetExpiredLotToDepletedWhenQuantityReachesZero()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        item.AdjustStock(
            lotId,
            100,
            false,
            "user-001",
            "Expired stock correction",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(0, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Depleted, lot.Status);
    }

    [Fact]
    public void QuarantineLot_ShouldChangeAvailableLotStatus()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.QuarantineLot(lotId, ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(InventoryLotStatus.Quarantined, lot.Status);
        Assert.Equal(100, lot.AvailableQuantity);
    }

    [Fact]
    public void QuarantineLot_ShouldAllowExpiredLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        item.QuarantineLot(lotId, ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(InventoryLotStatus.Quarantined, lot.Status);
    }

    [Fact]
    public void ReleaseLotFromQuarantine_ShouldRestoreAvailableStatus()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.QuarantineLot(lotId, ReceivedAt.AddHours(1));

        item.ReleaseLotFromQuarantine(
            lotId,
            CurrentDate,
            ReceivedAt.AddHours(2));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(InventoryLotStatus.Available, lot.Status);
    }

    [Fact]
    public void ReleaseLotFromQuarantine_ShouldRestoreExpiredStatus()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        item.QuarantineLot(lotId, ReceivedAt.AddHours(1));

        item.ReleaseLotFromQuarantine(
            lotId,
            CurrentDate,
            ReceivedAt.AddHours(2));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(InventoryLotStatus.Expired, lot.Status);
    }

   
    [Fact]
    public void DisposeLot_ShouldSetStatusToDisposedAndClearQuantity()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.DisposeLot(
            lotId,
            "user-001",
            "Contamination",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(0, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Disposed, lot.Status);
    }

    [Fact]
    public void DisposeLot_ShouldAllowDisposingExpiredLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(
            item,
            expiryDate: CurrentDate.AddDays(-1));

        item.DisposeLot(
            lotId,
            "user-001",
            "Expired material",
            ReceivedAt.AddHours(1));

        var lot = Assert.Single(item.Lots);

        Assert.Equal(0, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Disposed, lot.Status);
    }

    [Fact]
    public void QuarantineLot_ShouldRejectDepletedLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.AdjustStock(
            lotId,
            100,
            false,
            "user-001",
            "Stock correction",
            ReceivedAt.AddHours(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.QuarantineLot(lotId, ReceivedAt.AddHours(2)));
    }

    [Fact]
    public void ConsumeStock_ShouldRejectQuarantinedLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.QuarantineLot(lotId, ReceivedAt.AddHours(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.ConsumeStock(
                lotId,
                10,
                "user-002",
                null,
                null,
                CurrentDate,
                ReceivedAt.AddHours(2)));
    }

    [Fact]
    public void AdjustStock_ShouldRejectQuarantinedLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.QuarantineLot(lotId, ReceivedAt.AddHours(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.AdjustStock(
                lotId,
                10,
                true,
                "user-001",
                "Adjustment",
                ReceivedAt.AddHours(2)));
    }

    [Fact]
    public void DisposeLot_ShouldRejectAlreadyDisposedLot()
    {
        var item = CreateItem();
        var lotId = ReceiveLot(item);

        item.DisposeLot(
            lotId,
            "user-001",
            "Disposal",
            ReceivedAt.AddHours(1));

        Assert.Throws<InventoryDomainException>(() =>
            item.DisposeLot(
                lotId,
                "user-001",
                "Second disposal",
                ReceivedAt.AddHours(2)));
    }

    [Fact]
    public void InventoryItem_ShouldRejectUnknownLotId()
    {
        var item = CreateItem();

        Assert.Throws<InventoryDomainException>(() =>
            item.QuarantineLot(Guid.NewGuid(), ReceivedAt));
    }
}
