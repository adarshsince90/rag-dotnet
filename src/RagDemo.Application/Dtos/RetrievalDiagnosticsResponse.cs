public record RetrievalDiagnosticsResponse(
    int TotalChunks,
    int QualifiedChunks,
    int ReturnedChunks,
    int TopK,
    long RetrievalMs = 0,
    long GenerationMs = 0,
    int TotalMs = 0,
    int TotalTokens = 0,
    bool llmInvoked = false);