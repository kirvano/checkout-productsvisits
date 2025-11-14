namespace Checkout.ProductsVisits.Domain.Aggregate.Base.Event;

public record EventMetadata
{
    public Guid EventId { get; init; }
    public string EventType { get; init; }
    public string EventVersion { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public EventLineage Lineage { get; init; }

    public EventMetadata(string eventType, string eventVersion = "1.0", EventLineage? lineage = null)
    {
        EventId = Guid.NewGuid();
        EventType = eventType;
        EventVersion = eventVersion;
        Timestamp = DateTimeOffset.UtcNow;
        Lineage = lineage ?? EventLineage.Empty;
    }

    private EventMetadata(Guid eventId, string eventType, string eventVersion, DateTimeOffset timestamp, EventLineage lineage)
    {
        EventId = eventId;
        EventType = eventType;
        EventVersion = eventVersion;
        Timestamp = timestamp;
        Lineage = lineage;
    }

    public EventMetadata WithLineage(EventLineage lineage) =>
        new(EventId, EventType, EventVersion, Timestamp, lineage);

    public EventMetadata WithTraceId(string traceId) =>
        WithLineage(Lineage.WithTraceId(traceId));
}