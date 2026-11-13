
using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Infrastructure.Messaging;

public sealed class DomainEventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(
        TEvent domainEvent,
        CancellationToken cancellationToken = default)
        where TEvent : notnull
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        if (domainEvent is not IDomainEvent)
        {
            throw new ArgumentException(
                "The event must implement IDomainEvent.",
                nameof(domainEvent));
        }

        var handlers = _serviceProvider
            .GetServices<IDomainEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(
                domainEvent,
                cancellationToken);
        }
    }
}
