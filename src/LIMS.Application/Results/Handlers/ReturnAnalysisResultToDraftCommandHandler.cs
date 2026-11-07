using LIMS.Application.Abstractions.Persistence;

namespace LIMS.Application.Results.Handlers;

public sealed class ReturnAnalysisResultToDraftCommandHandler
    : ICommandHandler<ReturnAnalysisResultToDraftCommand, bool>
{
    private readonly IAnalysisResultRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReturnAnalysisResultToDraftCommandHandler(
        IAnalysisResultRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ReturnAnalysisResultToDraftCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetWithDetailsAsync(
            command.ResultId,
            cancellationToken);

        if (result is null)
            return false;

        result.ReturnToDraft(
            command.RequestedByUserId,
            _dateTimeProvider.UtcNow);

        _repository.Update(result);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
