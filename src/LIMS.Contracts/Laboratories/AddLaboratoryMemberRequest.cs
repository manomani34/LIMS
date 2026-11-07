namespace LIMS.Contracts.Laboratories;

public sealed record AddLaboratoryMemberRequest(
    string UserId,
    LaboratoryMemberRoleContract Role);
