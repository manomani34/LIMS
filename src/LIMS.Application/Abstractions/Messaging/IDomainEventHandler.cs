
namespace LIMS.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent>
    where TEvent : notnull
{
    Task HandleAsync(
        TEvent domainEvent,
        CancellationToken cancellationToken = default);
}
