public sealed record StreamingEvent
{
    public required string EventType { get; init; }
    public required object Payload { get; init; }
}