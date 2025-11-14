namespace Checkout.ProductsVisits.Domain.Aggregate.Base.Event;

public class Envelope<TEvent> where TEvent : EventBase
{
    public required EventMetadata Metadata { get; init; }
    public required TEvent Data { get; init; }
}