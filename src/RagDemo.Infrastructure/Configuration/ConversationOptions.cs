namespace RagDemo.Infrastructure.Configuration;

public sealed class ConversationOptions
{
    public const string SectionName = "Conversation";

    public int MaxHistoryTurns { get; set; } = 4;
}