using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Application.Laboratories.Commands;

public sealed record AddLaboratoryMemberCommand(
    Guid LaboratoryId,
    string UserId,
    LaboratoryMemberRole Role) : ICommand<bool>;
