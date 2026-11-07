using LIMS.Domain.Common;

namespace LIMS.Domain.Files.Events;

public sealed record StoredFileUploadedDomainEvent(
    Guid FileId,
    DateTime OccurredOn) : IDomainEvent;
