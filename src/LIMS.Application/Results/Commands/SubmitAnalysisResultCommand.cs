using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record SubmitAnalysisResultCommand(
    Guid ResultId,
    string SubmittedByUserId) : ICommand<bool>;
