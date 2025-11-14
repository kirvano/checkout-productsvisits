using Checkout.ProductsVisits.Domain.Aggregate.Base.Entity;
using Checkout.ProductsVisits.Domain.Aggregate.Base.Event;

namespace Checkout.ProductsVisits.Domain.Aggregate.Base;


public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey> where TKey : struct
{
    public IReadOnlyCollection<EventBase> DomainEvents => _domainEvents.AsReadOnly();

    private readonly List<EventBase> _domainEvents = [];

    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(TKey id) : base(id)
    {
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void RaiseDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : EventBase
    {
        _domainEvents.Add(domainEvent);
    }
}