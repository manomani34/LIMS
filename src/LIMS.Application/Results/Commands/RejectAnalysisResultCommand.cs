using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record RejectAnalysisResultCommand(
    Guid ResultId,
    string ReviewerUserId,
    string Reason) : ICommand<bool>;

