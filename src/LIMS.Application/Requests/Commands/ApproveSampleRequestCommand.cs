using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Requests.Commands;

public sealed record ApproveSampleRequestCommand(
    Guid RequestId,
    string ApprovedByUserId,
    Guid LaboratoryId,
    string? Comments) : ICommand<bool>;
