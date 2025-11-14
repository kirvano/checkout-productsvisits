namespace Checkout.ProductsVisits.Domain.Aggregate.Base.Event;


public abstract class EventBase
{
    public EventMetadata Metadata { get; protected init; }
    public abstract object Data { get; }

    protected EventBase(string eventType, string eventVersion = "1.0", EventLineage? lineage = null)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));
        
        if (string.IsNullOrWhiteSpace(eventVersion))
            throw new ArgumentException("Event version cannot be null or empty", nameof(eventVersion));

        Metadata = new EventMetadata(eventType, eventVersion, lineage);
    }

    protected EventBase(EventMetadata metadata)
    {
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    public Guid Id => Metadata.EventId;
    public DateTime OccurredOn => Metadata.Timestamp.DateTime;
    public DateTimeOffset Timestamp => Metadata.Timestamp;
    public string EventType => Metadata.EventType;
    public string EventVersion => Metadata.EventVersion;
    public EventLineage Lineage => Metadata.Lineage;

    public EventBase WithLineage(EventLineage lineage)
    {
        if (lineage == null) throw new ArgumentNullException(nameof(lineage));
        
        var newMetadata = Metadata.WithLineage(lineage);
        return CreateWithNewMetadata(newMetadata);
    }

    public EventBase WithTraceId(string traceId)
    {
        if (string.IsNullOrWhiteSpace(traceId))
            throw new ArgumentException("Trace ID cannot be null or empty", nameof(traceId));
            
        var newMetadata = Metadata.WithTraceId(traceId);
        return CreateWithNewMetadata(newMetadata);
    }

    protected abstract EventBase CreateWithNewMetadata(EventMetadata metadata);

    public virtual string GetEventTypeName()
    {
        return GetType().Name.Replace("Event", "").ToLowerInvariant();
    }

    public virtual Dictionary<string, object> ToEventPayload()
    {
        return new Dictionary<string, object>
        {
            ["metadata"] = new
            {
                eventId = Metadata.EventId,
                eventType = Metadata.EventType,
                eventVersion = Metadata.EventVersion,
                timestamp = Metadata.Timestamp,
                lineage = new
                {
                    traceId = Metadata.Lineage.TraceId,
                    sourceSystem = Metadata.Lineage.SourceSystem,
                    targetSystem = Metadata.Lineage.TargetSystem,
                    correlationId = Metadata.Lineage.CorrelationId,
                    causationId = Metadata.Lineage.CausationId
                }
            },
            ["data"] = Data
        };
    }
}