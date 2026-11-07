using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Instruments.Abstractions;
using LIMS.Domain.Instruments;

namespace LIMS.Application.Instruments.Handlers;

public sealed class ScheduleInstrumentMaintenanceCommandHandler
    : ICommandHandler<Commands.ScheduleInstrumentMaintenanceCommand, Guid>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ScheduleInstrumentMaintenanceCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        Commands.ScheduleInstrumentMaintenanceCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken)
            ?? throw new InstrumentDomainException("Instrument was not found.");

        instrument.ScheduleMaintenance(
            command.Type,
            command.Description,
            command.ScheduledAt,
            command.ScheduledByUserId,
            _dateTimeProvider.UtcNow);

        var maintenanceId = instrument.MaintenanceRecords.Last().Id;

        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return maintenanceId;
    }
}

public sealed class CompleteInstrumentMaintenanceCommandHandler
    : ICommandHandler<Commands.CompleteInstrumentMaintenanceCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CompleteInstrumentMaintenanceCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.CompleteInstrumentMaintenanceCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(command.InstrumentId, cancellationToken);
        if (instrument is null) return false;

        instrument.CompleteMaintenance(
            command.MaintenanceId,
            command.CompletedByUserId,
            command.CompletionNotes,
            _dateTimeProvider.UtcNow);

        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
