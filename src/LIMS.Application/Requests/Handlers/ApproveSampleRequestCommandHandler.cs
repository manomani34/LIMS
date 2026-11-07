using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Requests.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Application.Requests.Handlers;

internal class ApproveSampleRequestCommandHandler
{
}
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Requests.Commands;
using LIMS.Application.Requests.Repositories;

namespace LIMS.Application.Requests.Handlers;

public sealed class ApproveSampleRequestCommandHandler
    : ICommandHandler<ApproveSampleRequestCommand, bool>
{
    private readonly ISampleRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApproveSampleRequestCommandHandler(
        ISampleRequestRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ApproveSampleRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = await _repository.GetWithDetailsAsync(
            command.RequestId,
            cancellationToken);

        if (request is null)
            return false;

        request.Approve(
            command.ApprovedByUserId,
            command.LaboratoryId,
            command.Comments,
            _dateTimeProvider.UtcNow);

        _repository.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
