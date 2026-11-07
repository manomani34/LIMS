using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Results.Commands;
using LIMS.Application.Results.Repositories;

namespace LIMS.Application.Results.Handlers;

public sealed class StartAnalysisResultReviewCommandHandler
    : ICommandHandler<StartAnalysisResultReviewCommand, bool>
{
    private readonly IAnalysisResultRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StartAnalysisResultReviewCommandHandler(
        IAnalysisResultRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        StartAnalysisResultReviewCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetWithDetailsAsync(
            command.ResultId,
            cancellationToken);

        if (result is null)
            return false;

        result.StartReview(
            command.ReviewerUserId,
            _dateTimeProvider.UtcNow);

        _repository.Update(result);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
