using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Files.Commands;

public sealed record CompleteFileUploadCommand(
    Guid FileId) : ICommand<bool>;
