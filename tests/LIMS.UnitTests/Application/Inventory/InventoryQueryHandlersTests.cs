
using LIMS.Application.Inventory.Handlers;
using LIMS.Application.Inventory.Queries;
using LIMS.Domain.Inventory.Enums;
using LIMS.UnitTests.Application.Inventory.Fakes;
using Xunit;

namespace LIMS.UnitTests.Application.Inventory;

public sealed class InventoryQueryHandlersTests
{
    private static FakeDateTimeProvider CreateClock()
    {
        return new FakeDateTimeProvider
        {
            UtcNow = InventoryTestData.CurrentUtc
        };
    }

    [Fact]
    public async Task GetById_ShouldReturnNullWhenItemDoesNotExist()
    {
        var repository = new FakeInventoryItemRepository();

        var handler = new GetInventoryItemByIdQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemByIdQuery(Guid.NewGuid()));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ShouldReturnItemDetails()
    {
        var item = InventoryTestData.CreateItem();

        InventoryTestData.ReceiveLot(
            item,
            "LOT-001",
            100,
            new DateOnly(2027, 12, 31));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var handler = new GetInventoryItemByIdQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemByIdQuery(item.Id));

        Assert.NotNull(result);

        Assert.Equal(item.Id, result.Item.Id);
        Assert.Equal(item.LaboratoryId, result.Item.LaboratoryId);
        Assert.Equal("CHEM-001", result.Item.ItemCode);
        Assert.Equal("Sodium Chloride", result.Item.Name);
        Assert.Equal(InventoryItemType.Chemical, result.Item.Type);
        Assert.Equal(InventoryItemStatus.Active, result.Item.Status);

        Assert.Equal(100, result.Item.TotalAvailableQuantity);

        var lot = Assert.Single(result.Lots);

        Assert.Equal("LOT-001", lot.LotNumber);
        Assert.Equal(100, lot.InitialQuantity);
        Assert.Equal(100, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);

        var movement = Assert.Single(result.Movements);

        Assert.Equal(InventoryMovementType.Receipt, movement.Type);
        Assert.Equal(100, movement.Quantity);
        Assert.Equal(100, movement.BalanceAfterMovement);
        Assert.Equal("user-001", movement.PerformedByUserId);
    }

    [Fact]
    public async Task GetById_ShouldMapExpiredLotUsingCurrentDate()
    {
        var item = InventoryTestData.CreateItem();

        // Lot در زمان دریافت منقضی نشده است؛
        // اما در تاریخ اجرای Query منقضی شده است.
        InventoryTestData.ReceiveLot(
            item,
            "LOT-EXPIRED",
            100,
            new DateOnly(2026, 11, 12));

        Assert.Equal(
            InventoryLotStatus.Available,
            Assert.Single(item.Lots).Status);

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var handler = new GetInventoryItemByIdQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemByIdQuery(item.Id));

        Assert.NotNull(result);

        var lot = Assert.Single(result.Lots);

        Assert.Equal(InventoryLotStatus.Expired, lot.Status);
        Assert.Equal(100, lot.AvailableQuantity);

        Assert.Equal(0, result.Item.TotalAvailableQuantity);
    }

    [Fact]
    public async Task GetById_ShouldThrowWhenQueryIsNull()
    {
        var handler = new GetInventoryItemByIdQueryHandler(
            new FakeInventoryItemRepository(),
            CreateClock());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.HandleAsync(null!));
    }

    [Fact]
    public async Task GetByCode_ShouldReturnNullWhenItemDoesNotExist()
    {
        var repository = new FakeInventoryItemRepository();

        var handler = new GetInventoryItemByCodeQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemByCodeQuery(
                Guid.NewGuid(),
                "UNKNOWN-001"));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCode_ShouldReturnMappedItem()
    {
        var laboratoryId = Guid.NewGuid();

        var item = InventoryTestData.CreateItem(
            laboratoryId,
            "CHEM-001");

        InventoryTestData.ReceiveLot(
            item,
            "LOT-001",
            75,
            new DateOnly(2027, 12, 31));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var handler = new GetInventoryItemByCodeQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemByCodeQuery(
                laboratoryId,
                "CHEM-001"));

        Assert.NotNull(result);

        Assert.Equal(item.Id, result.Id);
        Assert.Equal(laboratoryId, result.LaboratoryId);
        Assert.Equal("CHEM-001", result.ItemCode);
        Assert.Equal("Sodium Chloride", result.Name);
        Assert.Equal(75, result.TotalAvailableQuantity);

        Assert.Equal(laboratoryId, repository.LastLaboratoryId);
        Assert.Equal("CHEM-001", repository.LastItemCode);
    }

    [Fact]
    public async Task GetByCode_ShouldNotReturnItemFromAnotherLaboratory()
    {
        var item = InventoryTestData.CreateItem();

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var handler = new GetInventoryItemByCodeQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemByCodeQuery(
                Guid.NewGuid(),
                item.ItemCode));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCode_ShouldThrowWhenQueryIsNull()
    {
        var handler = new GetInventoryItemByCodeQueryHandler(
            new FakeInventoryItemRepository(),
            CreateClock());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.HandleAsync(null!));
    }

    [Fact]
    public async Task GetByLaboratory_ShouldReturnEmptyCollectionWhenNoItemsExist()
    {
        var handler = new GetInventoryItemsByLaboratoryQueryHandler(
            new FakeInventoryItemRepository(),
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemsByLaboratoryQuery(
                Guid.NewGuid(),
                null,
                null,
                1,
                20));

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByLaboratory_ShouldReturnMappedItems()
    {
        var laboratoryId = Guid.NewGuid();

        var firstItem = InventoryTestData.CreateItem(
            laboratoryId,
            "CHEM-001");

        var secondItem = InventoryTestData.CreateItem(
            laboratoryId,
            "CHEM-002",
            "Potassium Chloride",
            InventoryItemType.Reagent);

        InventoryTestData.ReceiveLot(
            firstItem,
            "LOT-001",
            100,
            new DateOnly(2027, 12, 31));

        InventoryTestData.ReceiveLot(
            secondItem,
            "LOT-002",
            50,
            new DateOnly(2027, 12, 31));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(firstItem, secondItem);

        var handler = new GetInventoryItemsByLaboratoryQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemsByLaboratoryQuery(
                laboratoryId,
                null,
                null,
                1,
                20));

        Assert.Equal(2, result.Count);

        Assert.Contains(result, x =>
            x.Id == firstItem.Id &&
            x.TotalAvailableQuantity == 100);

        Assert.Contains(result, x =>
            x.Id == secondItem.Id &&
            x.TotalAvailableQuantity == 50);

        Assert.Equal(laboratoryId, repository.LastLaboratoryId);
        Assert.Equal(1, repository.LastPageNumber);
        Assert.Equal(20, repository.LastPageSize);
    }

    [Fact]
    public async Task GetByLaboratory_ShouldPassStatusAndTypeFilters()
    {
        var laboratoryId = Guid.NewGuid();

        var activeChemical = InventoryTestData.CreateItem(
            laboratoryId,
            "CHEM-001",
            "Sodium Chloride",
            InventoryItemType.Chemical);

        var activeReagent = InventoryTestData.CreateItem(
            laboratoryId,
            "CHEM-002",
            "Potassium Chloride",
            InventoryItemType.Reagent);

        var inactiveChemical = InventoryTestData.CreateItem(
            laboratoryId,
            "CHEM-003",
            "Calcium Chloride",
            InventoryItemType.Chemical);

        inactiveChemical.Deactivate(
            InventoryTestData.CreatedAt.AddHours(1));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(
            activeChemical,
            activeReagent,
            inactiveChemical);

        var handler = new GetInventoryItemsByLaboratoryQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemsByLaboratoryQuery(
                laboratoryId,
                InventoryItemStatus.Active,
                InventoryItemType.Chemical,
                1,
                10));

        var item = Assert.Single(result);

        Assert.Equal(activeChemical.Id, item.Id);

        Assert.Equal(
            InventoryItemStatus.Active,
            repository.LastStatusFilter);

        Assert.Equal(
            InventoryItemType.Chemical,
            repository.LastTypeFilter);

        Assert.Equal(1, repository.LastPageNumber);
        Assert.Equal(10, repository.LastPageSize);
    }

    [Fact]
    public async Task GetByLaboratory_ShouldApplyPagination()
    {
        var laboratoryId = Guid.NewGuid();

        var items = Enumerable.Range(1, 5)
            .Select(index => InventoryTestData.CreateItem(
                laboratoryId,
                $"CHEM-{index:000}"))
            .ToArray();

        var repository = new FakeInventoryItemRepository();
        repository.Seed(items);

        var handler = new GetInventoryItemsByLaboratoryQueryHandler(
            repository,
            CreateClock());

        var result = await handler.HandleAsync(
            new GetInventoryItemsByLaboratoryQuery(
                laboratoryId,
                null,
                null,
                2,
                2));

        Assert.Equal(2, result.Count);

        Assert.Equal(items[2].Id, result.ElementAt(0).Id);
        Assert.Equal(items[3].Id, result.ElementAt(1).Id);

        Assert.Equal(2, repository.LastPageNumber);
        Assert.Equal(2, repository.LastPageSize);
    }

    [Fact]
    public async Task GetByLaboratory_ShouldThrowWhenQueryIsNull()
    {
        var handler = new GetInventoryItemsByLaboratoryQueryHandler(
            new FakeInventoryItemRepository(),
            CreateClock());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.HandleAsync(null!));
    }
}
