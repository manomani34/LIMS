namespace LIMS.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
