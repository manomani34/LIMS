using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Laboratories.Commands;

public sealed record UpdateLaboratoryInformationCommand(
    Guid LaboratoryId,
    string Name,
    string Code,
    string? Description) : ICommand<bool>;
