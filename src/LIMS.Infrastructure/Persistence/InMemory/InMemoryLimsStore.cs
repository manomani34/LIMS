using System.Collections.Concurrent;
using LIMS.Domain.Inventory;
using LIMS.Domain.Laboratories;

namespace LIMS.Infrastructure.Persistence.InMemory;

public sealed class InMemoryLimsStore
{
    private readonly ConcurrentDictionary<Guid, InventoryItem> _inventoryItems = new();
    private readonly ConcurrentDictionary<Guid, Laboratory> _laboratories = new();

    public IReadOnlyCollection<InventoryItem> GetInventoryItems()
    {
        return _inventoryItems.Values.ToArray();
    }

    public IReadOnlyCollection<Laboratory> GetLaboratories()
    {
        return _laboratories.Values.ToArray();
    }

    public IReadOnlyCollection<object> GetAggregateRoots()
    {
        return _inventoryItems.Values
            .Cast<object>()
            .Concat(_laboratories.Values)
            .ToArray();
    }

    public InventoryItem? GetInventoryItem(Guid id)
    {
        _inventoryItems.TryGetValue(id, out var item);
        return item;
    }

    public Laboratory? GetLaboratory(Guid id)
    {
        _laboratories.TryGetValue(id, out var laboratory);
        return laboratory;
    }

    public void AddInventoryItem(InventoryItem item)
    {
        if (!_inventoryItems.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException(
                $"Inventory item '{item.Id}' already exists.");
        }
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        _inventoryItems[item.Id] = item;
    }

    public void AddLaboratory(Laboratory laboratory)
    {
        if (!_laboratories.TryAdd(laboratory.Id, laboratory))
        {
            throw new InvalidOperationException(
                $"Laboratory '{laboratory.Id}' already exists.");
        }
    }

    public void UpdateLaboratory(Laboratory laboratory)
    {
        _laboratories[laboratory.Id] = laboratory;
    }

    public void RemoveLaboratory(Laboratory laboratory)
    {
        _laboratories.TryRemove(laboratory.Id, out _);
    }
}
