using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Requests.Commands;

public sealed record RejectSampleRequestCommand(
    Guid RequestId,
    string RejectedByUserId,
    string Reason) : ICommand<bool>;
