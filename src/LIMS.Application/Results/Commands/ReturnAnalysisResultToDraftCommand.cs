using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record ReturnAnalysisResultToDraftCommand(
    Guid ResultId,
    string RequestedByUserId) : ICommand<bool>;
