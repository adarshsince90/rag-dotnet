public sealed class ConversationTurn
{
    public string Question { get; init; } = string.Empty;

    public string Answer { get; init; } = string.Empty;

    public DateTimeOffset Timestamp { get; init; }
        = DateTimeOffset.UtcNow;
}