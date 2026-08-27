public sealed class GenerationDiagnostics
{
    public long GenerationMs { get; set; }

    public long TotalMs { get; set; }

    public int AnswerCharacters { get; set; }

    public int PromptTokens { get; set; }

    public int CompletionTokens { get; set; }
}