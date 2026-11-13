using LIMS.Application.Inventory.Repositories;
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.Infrastructure.Persistence.InMemory;

public sealed class InMemoryInventoryItemRepository
    : IInventoryItemRepository
{
    private readonly InMemoryLimsStore _store;

    public InMemoryInventoryItemRepository(InMemoryLimsStore store)
    {
        _store = store;
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = _store.GetInventoryItems().Any(item =>
            item.LaboratoryId == laboratoryId &&
            string.Equals(
                item.ItemCode,
                itemCode,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task<InventoryItem?> GetByIdAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(
            _store.GetInventoryItem(inventoryItemId));
    }

    public Task<InventoryItem?> GetWithDetailsAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(
            _store.GetInventoryItem(inventoryItemId));
    }

    public Task<InventoryItem?> GetByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var item = _store.GetInventoryItems().FirstOrDefault(x =>
            x.LaboratoryId == laboratoryId &&
            string.Equals(
                x.ItemCode,
                itemCode,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(item);
    }

    public Task<IReadOnlyCollection<InventoryItem>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        InventoryItemStatus? status,
        InventoryItemType? type,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<InventoryItem> items = _store
            .GetInventoryItems()
            .Where(x => x.LaboratoryId == laboratoryId)
            .Where(x => !status.HasValue || x.Status == status.Value)
            .Where(x => !type.HasValue || x.Type == type.Value)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult(items);
    }

    public Task AddAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _store.AddInventoryItem(item);

        return Task.CompletedTask;
    }

    public void Update(InventoryItem item)
    {
        _store.UpdateInventoryItem(item);
    }
}
