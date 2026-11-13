using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Requests.Repositories;
using LIMS.Domain.Requests;

namespace LIMS.Application.Requests.Handlers;

public sealed class CreateSampleRequestCommandHandler
    : ICommandHandler<CreateSampleRequestCommand, Guid>
{
    private readonly ISampleRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateSampleRequestCommandHandler(
        ISampleRequestRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        CreateSampleRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var exists = await _repository.ExistsByCodeAsync(
            command.RequestCode,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                $"A sample request with code '{command.RequestCode}' already exists.");

        var request = SampleRequest.Create(
            command.RequestCode,
            command.Title,
            command.Description,
            command.SampleName,
            command.SampleDescription,
            command.RequestedSampleType,
            command.SampleSource,
            command.RequestedByUserId,
            command.Priority,
            _dateTimeProvider.UtcNow.UtcDateTime);

        await _repository.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}
