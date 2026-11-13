using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Protocols.Commands;
using LIMS.Domain.Protocols;

namespace LIMS.Application.Protocols.Handlers;

public sealed class CreateProtocolCommandHandler
    : ICommandHandler<CreateProtocolCommand, Guid>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly ILaboratoryRepository _laboratoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateProtocolCommandHandler(
        IProtocolRepository protocolRepository,
        ILaboratoryRepository laboratoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _protocolRepository = protocolRepository;
        _laboratoryRepository = laboratoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        CreateProtocolCommand command,
        CancellationToken cancellationToken = default)
    {
        var laboratory = await _laboratoryRepository.GetByIdAsync(
            command.LaboratoryId,
            cancellationToken);

        if (laboratory is null)
            throw new InvalidOperationException(
                $"Laboratory '{command.LaboratoryId}' was not found.");

        if (await _protocolRepository.ExistsByCodeAsync(
                command.LaboratoryId,
                command.ProtocolCode,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Protocol code '{command.ProtocolCode}' already exists in this laboratory.");
        }

        var protocol = Protocol.Create(
            command.LaboratoryId,
            command.ProtocolCode,
            command.Name,
            command.Description,
            command.Version,
            command.CreatedByUserId,
            _dateTimeProvider.UtcNow);

        await _protocolRepository.AddAsync(protocol, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return protocol.Id;
    }
}

