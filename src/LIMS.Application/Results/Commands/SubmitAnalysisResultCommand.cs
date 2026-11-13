using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record SubmitAnalysisResultCommand(
    Guid ResultId,
    string SubmittedByUserId) : ICommand<bool>;

