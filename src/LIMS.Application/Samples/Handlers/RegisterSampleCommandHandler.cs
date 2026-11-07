using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Samples.Commands;
using LIMS.Application.Samples.Interfaces;
using LIMS.Domain.Samples;
using LIMS.Domain.Samples.Exceptions;

namespace LIMS.Application.Samples.Handlers;

public sealed class RegisterSampleCommandHandler
    : ICommandHandler<RegisterSampleCommand, Guid>
{
    private readonly ISampleRepository _sampleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterSampleCommandHandler(
        ISampleRepository sampleRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _sampleRepository = sampleRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        RegisterSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var exists = await _sampleRepository.ExistsByCodeAsync(
            command.LaboratoryId,
            command.SampleCode,
            cancellationToken);

        if (exists)
        {
            throw new SampleDomainException(
                $"Sample code '{command.SampleCode}' already exists in this laboratory.");
        }

        var sample = Sample.Register(
            command.LaboratoryId,
            command.SampleCode,
            command.Name,
            command.Description,
            command.Type,
            command.Priority,
            command.Source,
            command.RegisteredByUserId,
            _dateTimeProvider.UtcNow);

        await _sampleRepository.AddAsync(sample, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return sample.Id;
    }
}
