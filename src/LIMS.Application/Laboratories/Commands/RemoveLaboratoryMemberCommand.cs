using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Laboratories.Commands;

public sealed record RemoveLaboratoryMemberCommand(
    Guid LaboratoryId,
    string UserId) : ICommand<bool>;
