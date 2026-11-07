using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Experiments.Abstractions;
using LIMS.Application.Experiments.Commands;

namespace LIMS.Application.Experiments.Handlers;

public sealed class CancelExperimentCommandHandler
    : ICommandHandler<CancelExperimentCommand, bool>
{
    private readonly IExperimentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelExperimentCommandHandler(
        IExperimentRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        CancelExperimentCommand command,
        CancellationToken cancellationToken = default)
    {
        var experiment = await _repository.GetWithDetailsAsync(
            command.ExperimentId,
            cancellationToken);

        if (experiment is null)
            return false;

        experiment.Cancel(
            command.Reason,
            _dateTimeProvider.UtcNow);

        _repository.Update(experiment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
