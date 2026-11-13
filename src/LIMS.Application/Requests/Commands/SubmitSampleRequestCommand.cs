using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Requests.Commands;

public sealed record SubmitSampleRequestCommand(
    Guid RequestId,
    string SubmittedByUserId) : ICommand<bool>;
