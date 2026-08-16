public record RetrievalDiagnosticsResponse(
    int TotalChunks,
    int QualifiedChunks,
    int ReturnedChunks,
    int TopK);