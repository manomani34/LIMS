using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Laboratories.Commands;
using LIMS.Application.Laboratories.Interfaces;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class ChangeLaboratoryStatusCommandHandler
    : ICommandHandler<ChangeLaboratoryStatusCommand, bool>
{
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChangeLaboratoryStatusCommandHandler(
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ChangeLaboratoryStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.LaboratoryId == Guid.Empty)
            throw new ArgumentException("Laboratory identifier is required.");

        var laboratory = await _laboratoryRepository.GetByIdAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            throw new KeyNotFoundException("Laboratory was not found.");

        laboratory.ChangeStatus(
            command.Status,
            _dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
