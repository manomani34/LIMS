using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.Commands;

public sealed record LinkFileCommand(
    Guid FileId,
    FileReferenceType ReferenceType,
    Guid ReferenceId,
    string LinkedByUserId,
    string? Purpose) : ICommand<Guid>;
