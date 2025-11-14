using Checkout.ProductsVisits.Domain.Aggregate.Base.Event;

namespace Checkout.ProductsVisits.Domain.Aggregate.Base;

public interface IAggregateRoot<TKey> where TKey : struct
{
    TKey Id { get; }

    protected void RaiseDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : EventBase;
    public void ClearDomainEvents();
}