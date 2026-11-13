using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Requests.Repositories;

namespace LIMS.Application.Requests.Handlers;

public sealed class FulfillSampleRequestCommandHandler
    : ICommandHandler<FulfillSampleRequestCommand, bool>
{
    private readonly ISampleRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public FulfillSampleRequestCommandHandler(
        ISampleRequestRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        FulfillSampleRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = await _repository.GetWithDetailsAsync(
            command.RequestId,
            cancellationToken);

        if (request is null)
            return false;

        request.Fulfill(
            command.SampleId,
            _dateTimeProvider.UtcNow.UtcDateTime);

        _repository.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
