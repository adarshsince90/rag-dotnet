public sealed record GenerationDiagnostics
{
    public long GenerationMs { get; init; }
    public long TotalMs { get; init; }
    public int AnswerCharacters { get; init; }
    public int PromptTokens { get; init; }
    public int CompletionTokens { get; init; }
}
