using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Files.Commands;

public sealed record UnlinkFileCommand(
    Guid FileReferenceId) : ICommand<bool>;
