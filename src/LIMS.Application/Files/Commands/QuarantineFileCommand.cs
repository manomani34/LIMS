using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Files.Commands;

public sealed record QuarantineFileCommand(
    Guid FileId,
    string Reason) : ICommand<bool>;
