using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Requests.Repositories;

namespace LIMS.Application.Requests.Handlers;

public sealed class CancelSampleRequestCommandHandler
    : ICommandHandler<CancelSampleRequestCommand, bool>
{
    private readonly ISampleRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelSampleRequestCommandHandler(
        ISampleRequestRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        CancelSampleRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = await _repository.GetWithDetailsAsync(
            command.RequestId,
            cancellationToken);

        if (request is null)
            return false;

        request.Cancel(
            command.CancelledByUserId,
            command.Reason,
            _dateTimeProvider.UtcNow);

        _repository.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
