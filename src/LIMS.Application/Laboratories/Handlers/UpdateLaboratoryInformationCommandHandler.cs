using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Laboratories.Commands;
using LIMS.Application.Laboratories.Interfaces;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class UpdateLaboratoryInformationCommandHandler
    : ICommandHandler<UpdateLaboratoryInformationCommand, bool>
{
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateLaboratoryInformationCommandHandler(
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        UpdateLaboratoryInformationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.LaboratoryId == Guid.Empty)
            throw new ArgumentException("Laboratory identifier is required.");

        var normalizedCode = command.Code?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new ArgumentException("Laboratory code is required.");

        var laboratory = await _laboratoryRepository.GetByIdAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            throw new KeyNotFoundException("Laboratory was not found.");

        var codeExists = await _laboratoryRepository.ExistsByCodeAsync(
            normalizedCode,
            cancellationToken);

        if (codeExists &&
            !string.Equals(
                laboratory.Code,
                normalizedCode,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"A laboratory with code '{normalizedCode}' already exists.");
        }

        laboratory.UpdateInformation(
            command.Name,
            normalizedCode,
            command.Description,
            _dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
