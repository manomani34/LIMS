using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Files.Commands;

public sealed record ReleaseFileFromQuarantineCommand(
    Guid FileId) : ICommand<bool>;
