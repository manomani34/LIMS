using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record StartAnalysisResultReviewCommand(
    Guid ResultId,
    string ReviewerUserId) : ICommand<bool>;

