using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.Commands;
using LIMS.Application.Laboratories.Abstractions;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Samples.Abstractions;
using LIMS.Application.Samples.Interfaces;
using LIMS.Domain.Experiments;

namespace LIMS.Application.Experiments.Handlers;

public sealed class CreateExperimentCommandHandler
    : ICommandHandler<CreateExperimentCommand, Guid>
{
    private readonly IExperimentRepository _experimentRepository;
    private readonly ISampleRepository _sampleRepository;
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateExperimentCommandHandler(
        IExperimentRepository experimentRepository,
        ISampleRepository sampleRepository,
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _experimentRepository = experimentRepository;
        _sampleRepository = sampleRepository;
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        CreateExperimentCommand command,
        CancellationToken cancellationToken = default)
    {
        var laboratory = await _laboratoryRepository.GetByIdAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
        {
            throw new InvalidOperationException(
                $"Laboratory '{command.LaboratoryId}' was not found.");
        }

        var sample = await _sampleRepository.GetWithDetailsAsync(
            command.SampleId,
            cancellationToken);

        if (sample is null)
        {
            throw new InvalidOperationException(
                $"Sample '{command.SampleId}' was not found.");
        }

        if (sample.LaboratoryId != command.LaboratoryId)
        {
            throw new InvalidOperationException(
                "The sample does not belong to the specified laboratory.");
        }

        if (await _experimentRepository.ExistsByCodeAsync(
                command.LaboratoryId,
                command.ExperimentCode,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Experiment code '{command.ExperimentCode}' already exists in this laboratory.");
        }

        var experiment = Experiment.Create(
            command.LaboratoryId,
            command.SampleId,
            command.ExperimentCode,
            command.Title,
            command.Description,
            command.CreatedByUserId,
            _dateTimeProvider.UtcNow);

        await _experimentRepository.AddAsync(experiment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return experiment.Id;
    }
}
