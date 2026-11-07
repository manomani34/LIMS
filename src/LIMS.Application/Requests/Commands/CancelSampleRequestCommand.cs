using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Requests.Commands;

public sealed record CancelSampleRequestCommand(
    Guid RequestId,
    string CancelledByUserId,
    string? Reason) : ICommand<bool>;
