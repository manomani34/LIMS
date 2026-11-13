using System.Reflection;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.InMemory;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly LimsDbContext _context;
    private readonly IEventPublisher _eventPublisher;
    private readonly InMemoryLimsStore _store;

    private static readonly MethodInfo PublishMethod =
        typeof(InMemoryUnitOfWork).GetMethod(
            nameof(PublishTypedEventAsync),
            BindingFlags.Instance | BindingFlags.NonPublic)!;

    public InMemoryUnitOfWork(
        LimsDbContext context,
        IEventPublisher eventPublisher,
        InMemoryLimsStore store)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _store = store;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var aggregates = _context.ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
            .Concat(_store.GetAggregateRoots())
            .OfType<IDomainEventSource>()
            .Distinct()
            .ToList();

        var domainEvents = aggregates
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        var affectedRows = await _context.SaveChangesAsync(
            cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await PublishDomainEventAsync(
                domainEvent,
                cancellationToken);
        }

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return affectedRows;
    }

    private Task PublishDomainEventAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var method = PublishMethod.MakeGenericMethod(
            domainEvent.GetType());

        return (Task)method.Invoke(
            this,
            [domainEvent, cancellationToken])!;
    }

    private Task PublishTypedEventAsync<TEvent>(
        TEvent domainEvent,
        CancellationToken cancellationToken)
        where TEvent : notnull
    {
        return _eventPublisher.PublishAsync(
            domainEvent,
            cancellationToken);
    }
}
