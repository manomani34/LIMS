using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Laboratories.Commands;
using LIMS.Application.Laboratories.Interfaces;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class AddLaboratoryMemberCommandHandler
    : ICommandHandler<AddLaboratoryMemberCommand, bool>
{
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddLaboratoryMemberCommandHandler(
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        AddLaboratoryMemberCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.LaboratoryId == Guid.Empty)
            throw new ArgumentException("Laboratory identifier is required.");

        var laboratory = await _laboratoryRepository.GetWithMembersAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            throw new KeyNotFoundException("Laboratory was not found.");

        laboratory.AddMember(
            command.UserId,
            command.Role,
            _dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
