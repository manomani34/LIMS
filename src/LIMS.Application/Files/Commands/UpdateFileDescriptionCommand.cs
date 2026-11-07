using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Files.Commands;

public sealed record UpdateFileDescriptionCommand(
    Guid FileId,
    string? Description) : ICommand<bool>;
