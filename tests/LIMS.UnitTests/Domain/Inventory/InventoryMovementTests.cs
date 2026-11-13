
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;

namespace LIMS.UnitTests.Domain.Inventory;

public sealed class InventoryMovementTests
{
    private static readonly DateTime OccurredAt =
        new(2026, 11, 10, 10, 0, 0, DateTimeKind.Utc);

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
            OccurredAt);
    }

    private static Guid ReceiveStock(
        InventoryItem item,
        decimal quantity = 100)
    {
        return item.ReceiveStock(
            "LOT-001",
            "Supplier A",
            "Manufacturer A",
            quantity,
            "g",
            null,
            null,
            null,
            null,
            null,
            null,
            "user-001",
            "REF-001",
            "Initial receipt",
            OccurredAt);
    }

    [Fact]
    public void ReceiveStock_ShouldCreateReceiptMovement()
    {
        var item = CreateItem();

        var lotId = ReceiveStock(item);

        var movement = Assert.Single(item.Movements);

        Assert.NotEqual(Guid.Empty, movement.Id);
        Assert.Equal(lotId, movement.LotId);
        Assert.Equal(InventoryMovementType.Receipt, movement.Type);
        Assert.Equal(100, movement.Quantity);
        Assert.Equal(100, movement.BalanceAfterMovement);
        Assert.Equal("g", movement.Unit);
        Assert.Equal("user-001", movement.PerformedByUserId);
        Assert.Null(movement.ExperimentId);
        Assert.Equal("REF-001", movement.ReferenceNumber);
        Assert.Equal("Initial receipt", movement.Notes);
        Assert.Equal(OccurredAt, movement.OccurredAt);
    }

    [Fact]
    public void ConsumeStock_ShouldCreateConsumptionMovement()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);
        var experimentId = Guid.NewGuid();

        item.ConsumeStock(
            lotId,
            30,
            "user-002",
            experimentId,
            "Used in NMR experiment",
            DateOnly.FromDateTime(OccurredAt),
            OccurredAt.AddHours(1));

        var movement = item.Movements.Last();

        Assert.Equal(InventoryMovementType.Consumption, movement.Type);
        Assert.Equal(30, movement.Quantity);
        Assert.Equal(70, movement.BalanceAfterMovement);
        Assert.Equal(experimentId, movement.ExperimentId);
        Assert.Equal("user-002", movement.PerformedByUserId);
        Assert.Equal("Used in NMR experiment", movement.Notes);
        Assert.Equal(OccurredAt.AddHours(1), movement.OccurredAt);
    }

    [Fact]
    public void AdjustStock_ShouldCreateIncreaseMovement()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        item.AdjustStock(
            lotId,
            25,
            true,
            "user-002",
            "Inventory correction",
            OccurredAt.AddHours(1));

        var movement = item.Movements.Last();

        Assert.Equal(
            InventoryMovementType.AdjustmentIncrease,
            movement.Type);

        Assert.Equal(25, movement.Quantity);
        Assert.Equal(125, movement.BalanceAfterMovement);
        Assert.Equal("user-002", movement.PerformedByUserId);
        Assert.Equal("Inventory correction", movement.Notes);
        Assert.Null(movement.ExperimentId);
        Assert.Null(movement.ReferenceNumber);
    }

    [Fact]
    public void AdjustStock_ShouldCreateDecreaseMovement()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        item.AdjustStock(
            lotId,
            25,
            false,
            "user-002",
            "Inventory correction",
            OccurredAt.AddHours(1));

        var movement = item.Movements.Last();

        Assert.Equal(
            InventoryMovementType.AdjustmentDecrease,
            movement.Type);

        Assert.Equal(25, movement.Quantity);
        Assert.Equal(75, movement.BalanceAfterMovement);
    }

    [Fact]
    public void DisposeLot_ShouldCreateDisposalMovement()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        item.DisposeLot(
            lotId,
            "user-002",
            "Contaminated material",
            OccurredAt.AddHours(1));

        var movement = item.Movements.Last();

        Assert.Equal(InventoryMovementType.Disposal, movement.Type);
        Assert.Equal(100, movement.Quantity);
        Assert.Equal(0, movement.BalanceAfterMovement);
        Assert.Equal("user-002", movement.PerformedByUserId);
        Assert.Equal("Contaminated material", movement.Notes);
    }

    [Fact]
    public void DisposeLot_ShouldRejectDepletedLotWithoutCreatingMovement()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        item.AdjustStock(
            lotId,
            100,
            false,
            "user-001",
            "Stock correction",
            OccurredAt.AddHours(1));

        var movementCountBeforeDisposal = item.Movements.Count;

        Assert.Throws<InventoryDomainException>(() =>
            item.DisposeLot(
                lotId,
                "user-002",
                "Disposal",
                OccurredAt.AddHours(2)));

        Assert.Equal(
            movementCountBeforeDisposal,
            item.Movements.Count);
    }

    [Fact]
    public void MovementHistory_ShouldPreserveChronologicalOperations()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        item.ConsumeStock(
            lotId,
            20,
            "user-002",
            null,
            null,
            DateOnly.FromDateTime(OccurredAt),
            OccurredAt.AddHours(1));

        item.AdjustStock(
            lotId,
            10,
            true,
            "user-003",
            "Correction",
            OccurredAt.AddHours(2));

        var movements = item.Movements.ToList();

        Assert.Equal(3, movements.Count);

        Assert.Equal(
            InventoryMovementType.Receipt,
            movements[0].Type);

        Assert.Equal(
            InventoryMovementType.Consumption,
            movements[1].Type);

        Assert.Equal(
            InventoryMovementType.AdjustmentIncrease,
            movements[2].Type);

        Assert.Equal(100, movements[0].BalanceAfterMovement);
        Assert.Equal(80, movements[1].BalanceAfterMovement);
        Assert.Equal(90, movements[2].BalanceAfterMovement);
    }

    [Fact]
    public void MovementHistory_ShouldExposeReadOnlyCollection()
    {
        var item = CreateItem();
        ReceiveStock(item);

        Assert.IsAssignableFrom<IReadOnlyCollection<
            LIMS.Domain.Inventory.Entities.InventoryMovement>>(
            item.Movements);
    }

    [Fact]
    public void ConsumeStock_ShouldRejectEmptyExperimentId()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        Assert.Throws<InventoryDomainException>(() =>
            item.ConsumeStock(
                lotId,
                10,
                "user-002",
                Guid.Empty,
                null,
                DateOnly.FromDateTime(OccurredAt),
                OccurredAt.AddHours(1)));
    }

    [Fact]
    public void ConsumeStock_ShouldRejectEmptyUserId()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        Assert.Throws<InventoryDomainException>(() =>
            item.ConsumeStock(
                lotId,
                10,
                " ",
                null,
                null,
                DateOnly.FromDateTime(OccurredAt),
                OccurredAt.AddHours(1)));
    }

    [Fact]
    public void AdjustStock_ShouldRejectEmptyReason()
    {
        var item = CreateItem();
        var lotId = ReceiveStock(item);

        Assert.Throws<InventoryDomainException>(() =>
            item.AdjustStock(
                lotId,
                10,
                true,
                "user-001",
                " ",
                OccurredAt.AddHours(1)));
    }
}
