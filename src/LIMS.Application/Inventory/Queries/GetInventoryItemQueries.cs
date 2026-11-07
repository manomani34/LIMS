using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Inventory.DTOs;
using LIMS.Domain.Inventory.Enums;

namespace LIMS.Application.Inventory.Queries;

public sealed record GetInventoryItemByIdQuery(
    Guid InventoryItemId) : IQuery<InventoryItemDetailsDto?>;

public sealed record GetInventoryItemByCodeQuery(
    Guid LaboratoryId,
    string ItemCode) : IQuery<InventoryItemDto?>;

public sealed record GetInventoryItemsByLaboratoryQuery(
    Guid LaboratoryId,
    InventoryItemStatus? Status,
    InventoryItemType? Type,
    int PageNumber,
    int PageSize)
    : IQuery<IReadOnlyCollection<InventoryItemDto>>;
