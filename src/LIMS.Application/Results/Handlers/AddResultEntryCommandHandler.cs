using LIMS.Application.Results.Repositories;
using LIMS.Application.Results.Commands;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;

namespace LIMS.Application.Results.Handlers;

public sealed class AddResultEntryCommandHandler
    : ICommandHandler<AddResultEntryCommand, Guid>
{
    private readonly IAnalysisResultRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddResultEntryCommandHandler(
        IAnalysisResultRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        AddResultEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetWithDetailsAsync(
            command.ResultId,
            cancellationToken);

        if (result is null)
            throw new InvalidOperationException("Analysis result was not found.");

        result.AddEntry(
            command.ParameterName,
            command.ValueType,
            command.Value,
            command.NumericValue,
            command.Unit,
            command.Uncertainty,
            command.Method,
            command.Notes,
            _dateTimeProvider.UtcNow);

        var entryId = result.Entries.Last().Id;

        _repository.Update(result);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entryId;
    }
}

