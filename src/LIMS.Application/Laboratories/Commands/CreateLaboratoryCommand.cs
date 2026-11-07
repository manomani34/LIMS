using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Laboratories.Commands;

public sealed record CreateLaboratoryCommand(
    string Name,
    string Code,
    string? Description) : ICommand<Guid>;
