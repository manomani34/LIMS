using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record ArchiveAnalysisResultCommand(
    Guid ResultId,
    string ArchivedByUserId) : ICommand<bool>;
