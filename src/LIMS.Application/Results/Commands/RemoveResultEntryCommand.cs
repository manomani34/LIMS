using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record RemoveResultEntryCommand(
    Guid ResultId,
    Guid EntryId) : ICommand<bool>;
