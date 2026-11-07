using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Instruments.Abstractions;
using LIMS.Domain.Instruments;

namespace LIMS.Application.Instruments.Handlers;

public sealed class MarkInstrumentInUseCommandHandler
    : ICommandHandler<Commands.MarkInstrumentInUseCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MarkInstrumentInUseCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.MarkInstrumentInUseCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken);
        if (instrument is null) return false;

        instrument.MarkInUse(_dateTimeProvider.UtcNow);
        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class MarkInstrumentAvailableCommandHandler
    : ICommandHandler<Commands.MarkInstrumentAvailableCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MarkInstrumentAvailableCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.MarkInstrumentAvailableCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken);
        if (instrument is null) return false;

        instrument.MarkAvailable(_dateTimeProvider.UtcNow);
        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class MarkInstrumentOutOfServiceCommandHandler
    : ICommandHandler<Commands.MarkInstrumentOutOfServiceCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MarkInstrumentOutOfServiceCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.MarkInstrumentOutOfServiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken);
        if (instrument is null) return false;

        instrument.MarkOutOfService(command.Reason, _dateTimeProvider.UtcNow);
        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class RestoreInstrumentServiceCommandHandler
    : ICommandHandler<Commands.RestoreInstrumentServiceCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RestoreInstrumentServiceCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.RestoreInstrumentServiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken);
        if (instrument is null) return false;

        instrument.RestoreService(_dateTimeProvider.UtcNow);
        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class RetireInstrumentCommandHandler
    : ICommandHandler<Commands.RetireInstrumentCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RetireInstrumentCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.RetireInstrumentCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken);
        if (instrument is null) return false;

        instrument.Retire(_dateTimeProvider.UtcNow);
        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
