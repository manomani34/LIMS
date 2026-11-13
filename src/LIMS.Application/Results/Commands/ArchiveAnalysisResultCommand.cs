using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record ArchiveAnalysisResultCommand(
    Guid ResultId,
    string ArchivedByUserId) : ICommand<bool>;

