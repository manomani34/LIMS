using LIMS.Domain.Inventory;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.Repositories;

public interface IInventoryItemRepository
{
    Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default);

    Task<InventoryItem?> GetByIdAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default);

    Task<InventoryItem?> GetWithDetailsAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default);

    Task<InventoryItem?> GetByCodeAsync(
        Guid laboratoryId,
        string itemCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InventoryItem>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        InventoryItemStatus? status,
        InventoryItemType? type,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default);

    void Update(InventoryItem item);
}
