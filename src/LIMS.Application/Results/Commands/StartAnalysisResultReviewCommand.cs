using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record StartAnalysisResultReviewCommand(
    Guid ResultId,
    string ReviewerUserId) : ICommand<bool>;
