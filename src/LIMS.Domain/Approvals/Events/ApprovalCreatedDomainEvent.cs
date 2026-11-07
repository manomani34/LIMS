using LIMS.Domain.Approvals.Enums;
using LIMS.Domain.Common;

namespace LIMS.Domain.Approvals.Events;

public sealed record ApprovalCreatedDomainEvent(
    Guid ApprovalId,
    ApprovalSubjectType SubjectType,
    Guid SubjectId,
    string RequestedByUserId,
    DateTime OccurredOn) : IDomainEvent;
