using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pereprodai.Shared.Domain;

namespace Pereprodai.Shared.Infrastructure;

public class DomainEventDispatchInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public DomainEventDispatchInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await base.SavedChangesAsync(eventData, result, cancellationToken);
        var changes = eventData.Context?.ChangeTracker.Entries<AggregateRoot>().ToList();
        if(changes == null) return result;
        foreach (var entity in changes.Select(x => x.Entity))
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            entity.ClearDomainEvents(); // Можно сделать outbox, но лень :/
        }

        return result;
    }
}
