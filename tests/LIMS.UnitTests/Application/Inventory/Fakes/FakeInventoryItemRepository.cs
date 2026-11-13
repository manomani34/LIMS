
using LIMS.Application.Inventory.Repositories;
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.UnitTests.Application.Inventory.Fakes;

public sealed class FakeInventoryItemRepository : IInventoryItemRepository
{
    private readonly List<InventoryItem> _items = [];

    public IReadOnlyCollection<InventoryItem> Items => _items.AsReadOnly();

    public InventoryItem? AddedItem { get; private set; }

    public int AddCallCount { get; private set; }

    public int UpdateCallCount { get; private set; }

    public Guid? LastLaboratoryId { get; private set; }

    public string? LastItemCode { get; private set; }

    public Guid? LastInventoryItemId { get; private set; }

    public InventoryItemStatus? LastStatusFilter { get; private set; }

    public InventoryItemType? LastTypeFilter { get; private set; }

    public int LastPageNumber { get; private set; }

    public int LastPageSize { get; private set; }

    public void Seed(params InventoryItem[] items)
    {
        _items.AddRange(items);
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        LastLaboratoryId = laboratoryId;
        LastItemCode = itemCode;

        var exists = _items.Any(x =>
            x.LaboratoryId == laboratoryId &&
            string.Equals(
                x.ItemCode,
                itemCode,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task<InventoryItem?> GetByIdAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        LastInventoryItemId = inventoryItemId;

        return Task.FromResult(
            _items.FirstOrDefault(x => x.Id == inventoryItemId));
    }

    public Task<InventoryItem?> GetWithDetailsAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        LastInventoryItemId = inventoryItemId;

        return Task.FromResult(
            _items.FirstOrDefault(x => x.Id == inventoryItemId));
    }

    public Task<InventoryItem?> GetByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        LastLaboratoryId = laboratoryId;
        LastItemCode = itemCode;

        return Task.FromResult(
            _items.FirstOrDefault(x =>
                x.LaboratoryId == laboratoryId &&
                string.Equals(
                    x.ItemCode,
                    itemCode,
                    StringComparison.OrdinalIgnoreCase)));
    }

    public Task<IReadOnlyCollection<InventoryItem>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        InventoryItemStatus? status,
        InventoryItemType? type,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        LastLaboratoryId = laboratoryId;
        LastStatusFilter = status;
        LastTypeFilter = type;
        LastPageNumber = pageNumber;
        LastPageSize = pageSize;

        IEnumerable<InventoryItem> query = _items.Where(
            x => x.LaboratoryId == laboratoryId);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        var result = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<InventoryItem>>(result);
    }

    public Task AddAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default)
    {
        AddCallCount++;
        AddedItem = item;
        _items.Add(item);

        return Task.CompletedTask;
    }

    public void Update(InventoryItem item)
    {
        UpdateCallCount++;

        var index = _items.FindIndex(x => x.Id == item.Id);

        if (index >= 0)
            _items[index] = item;
        else
            _items.Add(item);
    }
}
