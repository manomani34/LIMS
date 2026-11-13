using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record RemoveResultEntryCommand(
    Guid ResultId,
    Guid EntryId) : ICommand<bool>;

