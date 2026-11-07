using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Inventory.DTOs;
using LIMS.Application.Inventory.Mapping;
using LIMS.Application.Inventory.Queries;
using LIMS.Application.Inventory.Repositories;

namespace LIMS.Application.Inventory.Handlers;

public sealed class GetInventoryItemByIdQueryHandler
    : IQueryHandler<GetInventoryItemByIdQuery, InventoryItemDetailsDto?>
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryItemByIdQueryHandler(
        IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryItemDetailsDto?> HandleAsync(
        GetInventoryItemByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var item = await _repository.GetWithDetailsAsync(
            query.InventoryItemId,
            cancellationToken);

        return item?.ToDetailsDto();
    }
}

public sealed class GetInventoryItemByCodeQueryHandler
    : IQueryHandler<GetInventoryItemByCodeQuery, InventoryItemDto?>
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryItemByCodeQueryHandler(
        IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryItemDto?> HandleAsync(
        GetInventoryItemByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var item = await _repository.GetByCodeAsync(
            query.LaboratoryId,
            query.ItemCode,
            cancellationToken);

        return item?.ToDto();
    }
}

public sealed class GetInventoryItemsByLaboratoryQueryHandler
    : IQueryHandler<
        GetInventoryItemsByLaboratoryQuery,
        IReadOnlyCollection<InventoryItemDto>>
{
    private readonly IInventoryItemRepository _repository;

    public GetInventoryItemsByLaboratoryQueryHandler(
        IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<InventoryItemDto>> HandleAsync(
        GetInventoryItemsByLaboratoryQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var items = await _repository.GetByLaboratoryIdAsync(
            query.LaboratoryId,
            query.Status,
            query.Type,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return items.Select(x => x.ToDto()).ToArray();
    }
}
