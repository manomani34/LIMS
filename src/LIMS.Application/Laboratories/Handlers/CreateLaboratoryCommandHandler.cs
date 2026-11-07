using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Laboratories.Commands;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Domain.Laboratories;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class CreateLaboratoryCommandHandler
    : ICommandHandler<CreateLaboratoryCommand, Guid>
{
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateLaboratoryCommandHandler(
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        CreateLaboratoryCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var normalizedCode = command.Code?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new ArgumentException("Laboratory code is required.");

        if (await _laboratoryRepository.ExistsByCodeAsync(
                normalizedCode,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A laboratory with code '{normalizedCode}' already exists.");
        }

        var laboratory = Laboratory.Create(
            command.Name,
            normalizedCode,
            command.Description,
            _dateTimeProvider.UtcNow);

        await _laboratoryRepository.AddAsync(
            laboratory,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return laboratory.Id;
    }
}
