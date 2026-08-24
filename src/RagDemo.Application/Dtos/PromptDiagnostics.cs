public sealed record PromptDiagnostics(
    int ContextCharacters,
    int PromptCharacters,
    int RetrievedChunkCount);