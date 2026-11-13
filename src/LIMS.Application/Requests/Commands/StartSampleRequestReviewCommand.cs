using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Requests.Commands;

public sealed record StartSampleRequestReviewCommand(
    Guid RequestId,
    string ReviewerUserId) : ICommand<bool>;
