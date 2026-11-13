using LIMS.Application.Instruments.Repositories;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Results.Commands;
using LIMS.Application.Results.Repositories;
using LIMS.Application.Samples.Interfaces;
using LIMS.Domain.Results;

namespace LIMS.Application.Results.Handlers;

public sealed class CreateAnalysisResultCommandHandler
    : ICommandHandler<CreateAnalysisResultCommand, Guid>
{
    private readonly IAnalysisResultRepository _resultRepository;
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IExperimentRepository _experimentRepository;
    private readonly ISampleRepository _sampleRepository;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateAnalysisResultCommandHandler(
        IAnalysisResultRepository resultRepository,
        ILaboratoryRepository laboratoryRepository,
        IExperimentRepository experimentRepository,
        ISampleRepository sampleRepository,
        IInstrumentRepository instrumentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _resultRepository = resultRepository;
        _laboratoryRepository = laboratoryRepository;
        _experimentRepository = experimentRepository;
        _sampleRepository = sampleRepository;
        _instrumentRepository = instrumentRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        CreateAnalysisResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var laboratory = await _laboratoryRepository.GetByIdAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            throw new InvalidOperationException("Laboratory was not found.");

        var experiment = await _experimentRepository.GetWithDetailsAsync(
            command.ExperimentId,
            cancellationToken);

        if (experiment is null)
            throw new InvalidOperationException("Experiment was not found.");

        if (experiment.LaboratoryId != command.LaboratoryId)
            throw new InvalidOperationException(
                "The experiment does not belong to the specified laboratory.");

        if (experiment.SampleId != command.SampleId)
            throw new InvalidOperationException(
                "The experiment is not associated with the specified sample.");

        var sample = await _sampleRepository.GetWithDetailsAsync(
            command.SampleId,
            cancellationToken);

        if (sample is null)
            throw new InvalidOperationException("Sample was not found.");

        if (sample.LaboratoryId != command.LaboratoryId)
            throw new InvalidOperationException(
                "The sample does not belong to the specified laboratory.");

        if (command.InstrumentId.HasValue)
        {
            var instrument = await _instrumentRepository.GetWithDetailsAsync(
                command.InstrumentId.Value,
                cancellationToken);

            if (instrument is null)
                throw new InvalidOperationException("Instrument was not found.");

            if (instrument.LaboratoryId != command.LaboratoryId)
                throw new InvalidOperationException(
                    "The instrument does not belong to the specified laboratory.");
        }

        var codeExists = await _resultRepository.ExistsByCodeAsync(
            command.LaboratoryId,
            command.ResultCode,
            cancellationToken);

        if (codeExists)
            throw new InvalidOperationException(
                "An analysis result with the specified code already exists.");

        var result = AnalysisResult.Create(
            command.LaboratoryId,
            command.ExperimentId,
            command.SampleId,
            command.InstrumentId,
            command.ResultCode,
            command.Title,
            command.Description,
            command.CreatedByUserId,
            _dateTimeProvider.UtcNow);

        await _resultRepository.AddAsync(result, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Id;
    }
}


