using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Instruments.Repositories;
using LIMS.Domain.Instruments;

namespace LIMS.Application.Instruments.Handlers;

public sealed class UpdateInstrumentInformationCommandHandler
    : ICommandHandler<Commands.UpdateInstrumentInformationCommand, bool>
{
    private readonly IInstrumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateInstrumentInformationCommandHandler(
        IInstrumentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        Commands.UpdateInstrumentInformationCommand command,
        CancellationToken cancellationToken = default)
    {
        var instrument = await _repository.GetWithDetailsAsync(
            command.InstrumentId,
            cancellationToken);

        if (instrument is null)
            return false;

        instrument.UpdateInformation(
            command.Name,
            command.Type,
            command.Manufacturer,
            command.Model,
            command.SerialNumber,
            command.Location,
            command.Description,
            _dateTimeProvider.UtcNow);

        _repository.Update(instrument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

