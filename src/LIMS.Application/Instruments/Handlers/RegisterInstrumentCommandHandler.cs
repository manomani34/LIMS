using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Instruments.Abstractions;
using LIMS.Domain.Instruments;

namespace LIMS.Application.Instruments.Handlers;

public sealed class RegisterInstrumentCommandHandler
    : ICommandHandler<Commands.RegisterInstrumentCommand, Guid>
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterInstrumentCommandHandler(
        IInstrumentRepository instrumentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _instrumentRepository = instrumentRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        Commands.RegisterInstrumentCommand command,
        CancellationToken cancellationToken = default)
    {
        var exists = await _instrumentRepository.ExistsByCodeAsync(
            command.LaboratoryId,
            command.InstrumentCode,
            cancellationToken);

        if (exists)
            throw new InstrumentDomainException("An instrument with this code already exists in the laboratory.");

        var instrument = Instrument.Register(
            command.LaboratoryId,
            command.InstrumentCode,
            command.Name,
            command.Type,
            command.Manufacturer,
            command.Model,
            command.SerialNumber,
            command.Location,
            command.Description,
            _dateTimeProvider.UtcNow);

        await _instrumentRepository.AddAsync(instrument, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return instrument.Id;
    }
}
