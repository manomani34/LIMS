using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Samples.Commands;
using LIMS.Application.Samples.Interfaces;

namespace LIMS.Application.Samples.Handlers;

public sealed class UnassignSampleCommandHandler
    : ICommandHandler<UnassignSampleCommand, bool>
{
    private readonly ISampleRepository _sampleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnassignSampleCommandHandler(
        ISampleRepository sampleRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _sampleRepository = sampleRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        UnassignSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var sample = await _sampleRepository.GetByIdAsync(
            command.SampleId,
            cancellationToken);

        if (sample is null)
            return false;

        sample.Unassign(_dateTimeProvider.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
