using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Files.Commands;

public sealed record DeleteFileCommand(
    Guid FileId,
    string DeletedByUserId) : ICommand<bool>;
