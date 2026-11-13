using LIMS.Application.Instruments.Repositories;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Instruments;
using LIMS.Application.Results.Commands;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers;

public sealed class UpdateAnalysisResultInformationCommandHandler
    : ICommandHandler<UpdateAnalysisResultInformationCommand, bool>
{
    private readonly IAnalysisResultRepository _repository;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateAnalysisResultInformationCommandHandler(
        IAnalysisResultRepository repository,
        IInstrumentRepository instrumentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _instrumentRepository = instrumentRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        UpdateAnalysisResultInformationCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetWithDetailsAsync(
            command.ResultId,
            cancellationToken);

        if (result is null)
            return false;

        if (command.InstrumentId.HasValue)
        {
            var instrument = await _instrumentRepository.GetWithDetailsAsync(
                command.InstrumentId.Value,
                cancellationToken);

            if (instrument is null)
                throw new InvalidOperationException("Instrument was not found.");

            if (instrument.LaboratoryId != result.LaboratoryId)
                throw new InvalidOperationException(
                    "The instrument does not belong to the result laboratory.");
        }

        result.UpdateInformation(
            command.Title,
            command.Description,
            command.InstrumentId,
            _dateTimeProvider.UtcNow);

        _repository.Update(result);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

