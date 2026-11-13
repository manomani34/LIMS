using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record ReturnAnalysisResultToDraftCommand(
    Guid ResultId,
    string RequestedByUserId) : ICommand<bool>;

