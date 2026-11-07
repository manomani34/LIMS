using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Protocols.Abstractions;
using LIMS.Application.Protocols.Commands;

namespace LIMS.Application.Protocols.Handlers;

public sealed class ActivateProtocolCommandHandler
    : ICommandHandler<ActivateProtocolCommand, bool>
{
    private readonly IProtocolRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ActivateProtocolCommandHandler(
        IProtocolRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ActivateProtocolCommand command,
        CancellationToken cancellationToken = default)
    {
        var protocol = await _repository.GetWithDetailsAsync(
            command.ProtocolId,
            cancellationToken);

        if (protocol is null)
            return false;

        protocol.Activate(_dateTimeProvider.UtcNow);

        _repository.Update(protocol);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
