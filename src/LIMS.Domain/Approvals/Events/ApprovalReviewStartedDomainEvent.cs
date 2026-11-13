using LIMS.Domain.Common;
using LIMS.Domain.Approvals.Enums;

namespace LIMS.Domain.Approvals.Events;

public sealed record ApprovalReviewStartedDomainEvent(
    Guid ApprovalId,
    string ReviewerUserId,
    DateTime StartedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

