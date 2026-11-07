using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Handlers;

public sealed class AddProtocolStepCommandHandler
    : ICommandHandler<AddProtocolStepCommand, Guid>
{
    private readonly IProtocolRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddProtocolStepCommandHandler(
        IProtocolRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        AddProtocolStepCommand command,
        CancellationToken cancellationToken = default)
    {
        var protocol = await _repository.GetWithDetailsAsync(
            command.ProtocolId,
            cancellationToken);

        if (protocol is null)
            throw new InvalidOperationException(
                $"Protocol '{command.ProtocolId}' was not found.");

        var stepId = protocol.AddStep(
            command.Sequence,
            command.Title,
            command.Instruction,
            command.Type,
            command.IsRequired,
            command.TargetValue,
            command.TargetUnit,
            command.MinimumValue,
            command.MaximumValue,
            command.Notes,
            _dateTimeProvider.UtcNow);

        _repository.Update(protocol);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return stepId;
    }
}
