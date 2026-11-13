
using LIMS.Application.Inventory.Commands;
using LIMS.Application.Inventory.Handlers;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;
using LIMS.UnitTests.Application.Inventory.Fakes;
using Xunit;

namespace LIMS.UnitTests.Application.Inventory;

public sealed class InventoryCommandHandlersTests
{
    private static readonly DateTimeOffset FixedUtc =
        InventoryTestData.CurrentUtc;

    private static FakeDateTimeProvider CreateClock()
    {
        return new FakeDateTimeProvider { UtcNow = FixedUtc };
    }

    private static CreateInventoryItemCommand CreateCommand(
        Guid laboratoryId,
        string itemCode = "CHEM-001")
    {
        return new CreateInventoryItemCommand(
            laboratoryId,
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
            100);
    }

    [Fact]
    public async Task Create_ShouldAddItemAndSaveChanges()
    {
        var laboratory = InventoryTestData.CreateLaboratory();
        var repository = new FakeInventoryItemRepository();
        var laboratoryRepository = new FakeLaboratoryRepository();
        var unitOfWork = new FakeUnitOfWork();

        laboratoryRepository.Seed(laboratory);

        var handler = new CreateInventoryItemCommandHandler(
            repository,
            laboratoryRepository,
            unitOfWork,
            CreateClock());

        var id = await handler.HandleAsync(
            CreateCommand(laboratory.Id));

        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal(1, repository.AddCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);

        var item = Assert.Single(repository.Items);

        Assert.Equal(id, item.Id);
        Assert.Equal(laboratory.Id, item.LaboratoryId);
        Assert.Equal("CHEM-001", item.ItemCode);
        Assert.Equal("Sodium Chloride", item.Name);
        Assert.Equal(InventoryItemStatus.Active, item.Status);
        Assert.Equal(FixedUtc.UtcDateTime, item.CreatedAt);
        Assert.Same(item, repository.AddedItem);
    }

    [Fact]
    public async Task Create_ShouldThrowWhenLaboratoryDoesNotExist()
    {
        var repository = new FakeInventoryItemRepository();
        var laboratoryRepository = new FakeLaboratoryRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateInventoryItemCommandHandler(
            repository,
            laboratoryRepository,
            unitOfWork,
            CreateClock());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(
                CreateCommand(Guid.NewGuid())));

        Assert.Equal(0, repository.AddCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Create_ShouldRejectDuplicateItemCode()
    {
        var laboratory = InventoryTestData.CreateLaboratory();
        var existingItem = InventoryTestData.CreateItem(
            laboratory.Id,
            "CHEM-001");

        var repository = new FakeInventoryItemRepository();
        repository.Seed(existingItem);

        var laboratoryRepository = new FakeLaboratoryRepository();
        laboratoryRepository.Seed(laboratory);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateInventoryItemCommandHandler(
            repository,
            laboratoryRepository,
            unitOfWork,
            CreateClock());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(
                CreateCommand(laboratory.Id)));

        Assert.Equal(0, repository.AddCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Update_ShouldReturnFalseWhenItemDoesNotExist()
    {
        var repository = new FakeInventoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateInventoryItemCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new UpdateInventoryItemCommand(
            Guid.NewGuid(),
            "Updated Sodium Chloride",
            "Updated description",
            InventoryItemType.Reagent,
            "7647-14-5",
            "NaCl",
            "58.44",
            null,
            null,
            "g",
            200);

        var result = await handler.HandleAsync(command);

        Assert.False(result);
        Assert.Equal(0, repository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Update_ShouldUpdateItemAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateInventoryItemCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new UpdateInventoryItemCommand(
            item.Id,
            "Updated Sodium Chloride",
            "Updated description",
            InventoryItemType.Reagent,
            "7647-14-5",
            "NaCl",
            "58.44",
            "Non-hazardous",
            "SDS-001",
            "g",
            200);

        var result = await handler.HandleAsync(command);

        Assert.True(result);
        Assert.Equal("Updated Sodium Chloride", item.Name);
        Assert.Equal("Updated description", item.Description);
        Assert.Equal(InventoryItemType.Reagent, item.Type);
        Assert.Equal(200, item.MinimumStockLevel);
        Assert.Equal(FixedUtc.UtcDateTime, item.UpdatedAt);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Receive_ShouldCreateLotAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ReceiveInventoryStockCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new ReceiveInventoryStockCommand(
            item.Id,
            "LOT-RECEIVED",
            "Supplier B",
            "Manufacturer B",
            250,
            "g",
            null,
            null,
            new DateOnly(2027, 12, 31),
            "Shelf B",
            "Cool and dry",
            "COA-002",
            "user-002",
            "REF-002",
            "New stock");

        var lotId = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, lotId);

        var lot = Assert.Single(item.Lots);

        Assert.Equal(lotId, lot.Id);
        Assert.Equal("LOT-RECEIVED", lot.LotNumber);
        Assert.Equal(250, lot.AvailableQuantity);
        Assert.Equal(InventoryLotStatus.Available, lot.Status);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Receive_ShouldThrowWhenItemDoesNotExist()
    {
        var repository = new FakeInventoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ReceiveInventoryStockCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new ReceiveInventoryStockCommand(
            Guid.NewGuid(),
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
            null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(command));

        Assert.Equal(0, repository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Consume_ShouldReturnFalseWhenItemDoesNotExist()
    {
        var repository = new FakeInventoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConsumeInventoryStockCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new ConsumeInventoryStockCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            "user-001",
            null,
            null);

        var result = await handler.HandleAsync(command);

        Assert.False(result);
        Assert.Equal(0, repository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Consume_ShouldDecreaseStockAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var lotId = InventoryTestData.ReceiveLot(item);

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConsumeInventoryStockCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new ConsumeInventoryStockCommand(
            item.Id,
            lotId,
            25,
            "user-002",
            Guid.NewGuid(),
            "Test consumption");

        var result = await handler.HandleAsync(command);

        Assert.True(result);
        Assert.Equal(75, Assert.Single(item.Lots).AvailableQuantity);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);

        var movement = item.Movements.Last();

        Assert.Equal(InventoryMovementType.Consumption, movement.Type);
        Assert.Equal(25, movement.Quantity);
        Assert.Equal("user-002", movement.PerformedByUserId);
    }

    [Fact]
    public async Task Adjust_ShouldChangeStockAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var lotId = InventoryTestData.ReceiveLot(item);

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new AdjustInventoryStockCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new AdjustInventoryStockCommand(
            item.Id,
            lotId,
            20,
            true,
            "user-001",
            "Stock correction");

        var result = await handler.HandleAsync(command);

        Assert.True(result);
        Assert.Equal(120, Assert.Single(item.Lots).AvailableQuantity);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);

        Assert.Equal(
            InventoryMovementType.AdjustmentIncrease,
            item.Movements.Last().Type);
    }

    [Fact]
    public async Task Quarantine_ShouldChangeLotStatusAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var lotId = InventoryTestData.ReceiveLot(item);

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new QuarantineInventoryLotCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var result = await handler.HandleAsync(
            new QuarantineInventoryLotCommand(item.Id, lotId));

        Assert.True(result);
        Assert.Equal(
            InventoryLotStatus.Quarantined,
            Assert.Single(item.Lots).Status);

        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Release_ShouldRestoreLotStatusAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var lotId = InventoryTestData.ReceiveLot(item);

        item.QuarantineLot(
            lotId,
            InventoryTestData.CreatedAt.AddMinutes(1));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ReleaseInventoryLotCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var result = await handler.HandleAsync(
            new ReleaseInventoryLotCommand(item.Id, lotId));

        Assert.True(result);
        Assert.Equal(
            InventoryLotStatus.Available,
            Assert.Single(item.Lots).Status);

        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Dispose_ShouldDisposeLotAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var lotId = InventoryTestData.ReceiveLot(item);

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new DisposeInventoryLotCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new DisposeInventoryLotCommand(
            item.Id,
            lotId,
            "user-001",
            "Contamination");

        var result = await handler.HandleAsync(command);

        Assert.True(result);

        var lot = Assert.Single(item.Lots);

        Assert.Equal(InventoryLotStatus.Disposed, lot.Status);
        Assert.Equal(0, lot.AvailableQuantity);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Deactivate_ShouldChangeItemStatusAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new DeactivateInventoryItemCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var result = await handler.HandleAsync(
            new DeactivateInventoryItemCommand(item.Id));

        Assert.True(result);
        Assert.Equal(InventoryItemStatus.Inactive, item.Status);
        Assert.Equal(FixedUtc.UtcDateTime, item.UpdatedAt);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Reactivate_ShouldChangeItemStatusAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        item.Deactivate(InventoryTestData.CreatedAt.AddMinutes(1));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ReactivateInventoryItemCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var result = await handler.HandleAsync(
            new ReactivateInventoryItemCommand(item.Id));

        Assert.True(result);
        Assert.Equal(InventoryItemStatus.Active, item.Status);
        Assert.Equal(FixedUtc.UtcDateTime, item.UpdatedAt);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Discontinue_ShouldChangeItemStatusAndSaveChanges()
    {
        var item = InventoryTestData.CreateItem();
        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new DiscontinueInventoryItemCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var result = await handler.HandleAsync(
            new DiscontinueInventoryItemCommand(item.Id));

        Assert.True(result);
        Assert.Equal(InventoryItemStatus.Discontinued, item.Status);
        Assert.Equal(FixedUtc.UtcDateTime, item.UpdatedAt);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Theory]
    [InlineData("Deactivate")]
    [InlineData("Reactivate")]
    [InlineData("Discontinue")]
    public async Task StatusHandlers_ShouldReturnFalseWhenItemDoesNotExist(
        string operation)
    {
        var repository = new FakeInventoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();
        var clock = CreateClock();

        bool result;

        switch (operation)
        {
            case "Deactivate":
                result = await new DeactivateInventoryItemCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new DeactivateInventoryItemCommand(Guid.NewGuid()));
                break;

            case "Reactivate":
                result = await new ReactivateInventoryItemCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new ReactivateInventoryItemCommand(Guid.NewGuid()));
                break;

            default:
                result = await new DiscontinueInventoryItemCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new DiscontinueInventoryItemCommand(Guid.NewGuid()));
                break;
        }

        Assert.False(result);
        Assert.Equal(0, repository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Consume_ShouldNotSaveWhenDomainOperationFails()
    {
        var item = InventoryTestData.CreateItem();
        var lotId = InventoryTestData.ReceiveLot(
            item,
            expiryDate: new DateOnly(2026, 11, 12));

        var repository = new FakeInventoryItemRepository();
        repository.Seed(item);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ConsumeInventoryStockCommandHandler(
            repository,
            unitOfWork,
            CreateClock());

        var command = new ConsumeInventoryStockCommand(
            item.Id,
            lotId,
            10,
            "user-002",
            null,
            null);

        await Assert.ThrowsAsync<InventoryDomainException>(() =>
            handler.HandleAsync(command));

        Assert.Equal(0, repository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }


    [Theory]
    [InlineData("Adjust")]
    [InlineData("Quarantine")]
    [InlineData("Release")]
    [InlineData("Dispose")]
    public async Task LotHandlers_ShouldReturnFalseWhenItemDoesNotExist(
        string operation)
    {
        var repository = new FakeInventoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();
        var clock = CreateClock();

        var itemId = Guid.NewGuid();
        var lotId = Guid.NewGuid();

        bool result;

        switch (operation)
        {
            case "Adjust":
                result = await new AdjustInventoryStockCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new AdjustInventoryStockCommand(
                            itemId,
                            lotId,
                            10,
                            true,
                            "user-001",
                            "Test adjustment"));
                break;

            case "Quarantine":
                result = await new QuarantineInventoryLotCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new QuarantineInventoryLotCommand(
                            itemId,
                            lotId));
                break;

            case "Release":
                result = await new ReleaseInventoryLotCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new ReleaseInventoryLotCommand(
                            itemId,
                            lotId));
                break;

            default:
                result = await new DisposeInventoryLotCommandHandler(
                    repository, unitOfWork, clock)
                    .HandleAsync(
                        new DisposeInventoryLotCommand(
                            itemId,
                            lotId,
                            "user-001",
                            "Test disposal"));
                break;
        }

        Assert.False(result);
        Assert.Equal(0, repository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

}
