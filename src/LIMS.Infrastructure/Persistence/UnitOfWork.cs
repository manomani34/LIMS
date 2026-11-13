using System.Reflection;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly LimsDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    private static readonly MethodInfo PublishMethod =
        typeof(UnitOfWork).GetMethod(
            nameof(PublishTypedEventAsync),
            BindingFlags.Instance | BindingFlags.NonPublic)!;

    public UnitOfWork(
        LimsDbContext context,
        IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var aggregates = _context.ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
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
