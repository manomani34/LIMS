using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Inventory.DTOs;
using LIMS.Application.Inventory.Mapping;
using LIMS.Application.Inventory.Queries;
using LIMS.Application.Inventory.Repositories;

namespace LIMS.Application.Inventory.Handlers;

public sealed class GetInventoryItemByIdQueryHandler
    : IQueryHandler<GetInventoryItemByIdQuery, InventoryItemDetailsDto?>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetInventoryItemByIdQueryHandler(
        IInventoryItemRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<InventoryItemDetailsDto?> HandleAsync(
        GetInventoryItemByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var item = await _repository.GetWithDetailsAsync(
            query.InventoryItemId,
            cancellationToken);

        if (item is null)
            return null;

        var currentDate = DateOnly.FromDateTime(
            _dateTimeProvider.UtcNow.UtcDateTime);

        return item.ToDetailsDto(currentDate);
    }
}

public sealed class GetInventoryItemByCodeQueryHandler
    : IQueryHandler<GetInventoryItemByCodeQuery, InventoryItemDto?>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetInventoryItemByCodeQueryHandler(
        IInventoryItemRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
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

        if (item is null)
            return null;

        var currentDate = DateOnly.FromDateTime(
            _dateTimeProvider.UtcNow.UtcDateTime);

        return item.ToDto(currentDate);
    }
}

public sealed class GetInventoryItemsByLaboratoryQueryHandler
    : IQueryHandler<
        GetInventoryItemsByLaboratoryQuery,
        IReadOnlyCollection<InventoryItemDto>>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetInventoryItemsByLaboratoryQueryHandler(
        IInventoryItemRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
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

        var currentDate = DateOnly.FromDateTime(
            _dateTimeProvider.UtcNow.UtcDateTime);

        return items
            .Select(x => x.ToDto(currentDate))
            .ToArray();
    }
}
