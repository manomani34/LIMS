using LIMS.Application.Inventory.Repositories;
using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class InventoryItemRepository : IInventoryItemRepository
{
    private readonly LimsDbContext _context;
    private readonly DbSet<InventoryItem> _dbSet;

    public InventoryItemRepository(LimsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<InventoryItem>();
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ItemCode == itemCode,
            cancellationToken);
    }

    public Task<InventoryItem?> GetByIdAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(
            x => x.Id == inventoryItemId,
            cancellationToken);
    }

    public Task<InventoryItem?> GetWithDetailsAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(x => x.Lots)
            .Include(x => x.Movements)
            .FirstOrDefaultAsync(
                x => x.Id == inventoryItemId,
                cancellationToken);
    }

    public Task<InventoryItem?> GetByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(x => x.Lots)
            .FirstOrDefaultAsync(
                x => x.LaboratoryId == laboratoryId
                     && x.ItemCode == itemCode,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryItem>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        InventoryItemStatus? status,
        InventoryItemType? type,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(x => x.LaboratoryId == laboratoryId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        return await query
            .Include(x => x.Lots)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AddAsync(item, cancellationToken).AsTask();
    }

    public void Update(InventoryItem item)
    {
        _dbSet.Update(item);
    }
}
