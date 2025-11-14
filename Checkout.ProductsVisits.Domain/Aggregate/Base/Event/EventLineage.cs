namespace Checkout.ProductsVisits.Domain.Aggregate.Base.Event;

public record EventLineage
{
    public string? TraceId { get; init; }
    public string? SourceSystem { get; init; }
    public string? TargetSystem { get; init; }
    public string? CorrelationId { get; init; }
    public string? CausationId { get; init; }

    public EventLineage(
        string? traceId = null,
        string? sourceSystem = null,
        string? targetSystem = null,
        string? correlationId = null,
        string? causationId = null)
    {
        TraceId = traceId;
        SourceSystem = sourceSystem;
        TargetSystem = targetSystem;
        CorrelationId = correlationId;
        CausationId = causationId;
    }

    public EventLineage WithTraceId(string traceId) =>
        this with { TraceId = traceId };

    public EventLineage WithSource(string sourceSystem) =>
        this with { SourceSystem = sourceSystem };

    public EventLineage WithTarget(string targetSystem) =>
        this with { TargetSystem = targetSystem };

    public EventLineage WithCorrelation(string correlationId) =>
        this with { CorrelationId = correlationId };

    public EventLineage WithCausation(string causationId) =>
        this with { CausationId = causationId };

    public static EventLineage Empty => new();

    public static EventLineage ForGatewayRouter(string? traceId = null) =>
        new(traceId, "service:gateway-router", "topic:gateway_router.events");

    public static EventLineage ForPaymentOrchestrator(string? traceId = null) =>
        new(traceId, "service:payment-orchestrator", "topic:payment_orchestrator.events");
}