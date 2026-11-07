using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record RejectAnalysisResultCommand(
    Guid ResultId,
    string ReviewerUserId,
    string Reason) : ICommand<bool>;
