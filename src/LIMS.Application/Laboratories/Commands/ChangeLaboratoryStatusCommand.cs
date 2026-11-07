using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Application.Laboratories.Commands;

public sealed record ChangeLaboratoryStatusCommand(
    Guid LaboratoryId,
    LaboratoryStatus Status) : ICommand<bool>;
