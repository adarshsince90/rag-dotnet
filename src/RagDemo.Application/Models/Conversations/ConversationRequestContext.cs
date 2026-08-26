using RagDemo.Domain.Models;

public sealed record ConversationRequestContext
{
    public required string Prompt { get; init; }

    public required string Context { get; init; }

    public required string Question { get; init; }

    public required IReadOnlyCollection<MatchResponse> Matches { get; init; }

    public required IReadOnlyCollection<ConversationTurn> History { get; init; }

    public required ConversationDiagnostics ConversationDiagnostics
    {
        get;
        init;
    }

    public required RetrievalMetrics RetrievalMetrics
    {
        get;
        init;
    }

    public required PromptDiagnostics PromptDiagnostics
    {
        get;
        init;
    }
}