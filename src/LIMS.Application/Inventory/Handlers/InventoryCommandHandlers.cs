
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Inventory.Commands;
using LIMS.Application.Inventory.Repositories;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Domain.Inventory;

namespace LIMS.Application.Inventory.Handlers;

public sealed class CreateInventoryItemCommandHandler
    : ICommandHandler<CreateInventoryItemCommand, Guid>
{
    private readonly IInventoryItemRepository _repository;
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        CreateInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var laboratory = await _laboratoryRepository.GetByIdAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            throw new InvalidOperationException("Laboratory was not found.");

        if (await _repository.ExistsByCodeAsync(
                command.LaboratoryId,
                command.ItemCode,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Inventory item code '{command.ItemCode}' already exists.");
        }

        var now = _dateTimeProvider.UtcNow.UtcDateTime;

        var item = InventoryItem.Create(
            command.LaboratoryId,
            command.ItemCode,
            command.Name,
            command.Description,
            command.Type,
            command.CasNumber,
            command.ChemicalFormula,
            command.MolecularWeight,
            command.HazardClassification,
            command.SafetyDataSheetReference,
            command.DefaultUnit,
            command.MinimumStockLevel,
            now);

        await _repository.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.Id;
    }
}

public sealed class UpdateInventoryItemCommandHandler
    : ICommandHandler<UpdateInventoryItemCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        UpdateInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        item.UpdateInformation(
            command.Name,
            command.Description,
            command.Type,
            command.CasNumber,
            command.ChemicalFormula,
            command.MolecularWeight,
            command.HazardClassification,
            command.SafetyDataSheetReference,
            command.DefaultUnit,
            command.MinimumStockLevel,
            _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class ReceiveInventoryStockCommandHandler
    : ICommandHandler<ReceiveInventoryStockCommand, Guid>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReceiveInventoryStockCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        ReceiveInventoryStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            throw new InvalidOperationException("Inventory item was not found.");

        var lotId = item.ReceiveStock(
            command.LotNumber,
            command.Supplier,
            command.Manufacturer,
            command.Quantity,
            command.Unit,
            command.MinimumStockLevel,
            command.ManufactureDate,
            command.ExpiryDate,
            command.StorageLocation,
            command.StorageConditions,
            command.CertificateOfAnalysisNumber,
            command.PerformedByUserId,
            command.ReferenceNumber,
            command.Notes,
            _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return lotId;
    }
}

public sealed class ConsumeInventoryStockCommandHandler
    : ICommandHandler<ConsumeInventoryStockCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ConsumeInventoryStockCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ConsumeInventoryStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        var now = _dateTimeProvider.UtcNow.UtcDateTime;

        item.ConsumeStock(
            command.LotId,
            command.Quantity,
            command.ConsumedByUserId,
            command.ExperimentId,
            command.Notes,
            DateOnly.FromDateTime(now),
            now);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class AdjustInventoryStockCommandHandler
    : ICommandHandler<AdjustInventoryStockCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AdjustInventoryStockCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        AdjustInventoryStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        item.AdjustStock(
            command.LotId,
            command.Quantity,
            command.Increase,
            command.PerformedByUserId,
            command.Reason,
            _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class QuarantineInventoryLotCommandHandler
    : ICommandHandler<QuarantineInventoryLotCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public QuarantineInventoryLotCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        QuarantineInventoryLotCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        item.QuarantineLot(
            command.LotId,
            _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class ReleaseInventoryLotCommandHandler
    : ICommandHandler<ReleaseInventoryLotCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReleaseInventoryLotCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ReleaseInventoryLotCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        var now = _dateTimeProvider.UtcNow.UtcDateTime;

        item.ReleaseLotFromQuarantine(
            command.LotId,
            DateOnly.FromDateTime(now),
            now);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class DisposeInventoryLotCommandHandler
    : ICommandHandler<DisposeInventoryLotCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DisposeInventoryLotCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        DisposeInventoryLotCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWithDetailsAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        item.DisposeLot(
            command.LotId,
            command.PerformedByUserId,
            command.Reason,
            _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class DeactivateInventoryItemCommandHandler
    : ICommandHandler<DeactivateInventoryItemCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeactivateInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task<bool> HandleAsync(
        DeactivateInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        return ChangeStatusAsync(
            command.InventoryItemId,
            static (item, now) => item.Deactivate(now),
            cancellationToken);
    }

    private async Task<bool> ChangeStatusAsync(
        Guid id,
        Action<InventoryItem, DateTime> operation,
        CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);

        if (item is null)
            return false;

        operation(item, _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class ReactivateInventoryItemCommandHandler
    : ICommandHandler<ReactivateInventoryItemCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReactivateInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ReactivateInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        item.Reactivate(_dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class DiscontinueInventoryItemCommandHandler
    : ICommandHandler<DiscontinueInventoryItemCommand, bool>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DiscontinueInventoryItemCommandHandler(
        IInventoryItemRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        DiscontinueInventoryItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(
            command.InventoryItemId,
            cancellationToken);

        if (item is null)
            return false;

        item.Discontinue(_dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
