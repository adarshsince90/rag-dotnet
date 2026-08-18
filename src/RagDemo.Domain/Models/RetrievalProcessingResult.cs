public sealed class RetrievalProcessingResult
{
    public required IReadOnlyCollection<RetrievalResult> Results { get; init; }
    public int QualifiedChunks { get; init; }
    public int ReturnedChunks { get; init; }
    public int TopK { get; init; }
}