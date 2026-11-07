using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record ApproveAnalysisResultCommand(
    Guid ResultId,
    string ReviewerUserId,
    string? Comments) : ICommand<bool>;
