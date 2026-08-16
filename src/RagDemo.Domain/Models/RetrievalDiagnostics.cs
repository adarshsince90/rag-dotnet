// public record RetrievalDiagnostics(
//     int TotalChunks,
//     int QualifiedChunks,
//     int ReturnedChunks,
//     int TopK
// );
public sealed class RetrievalDiagnostics
{
    public int TotalChunks { get; init; }
    public int QualifiedChunks { get; init; }
    public int ReturnedChunks { get; init; }
    public int TopK { get; init; }
}