public sealed record RetrievalMetrics
{
    public long RetrievalMs { get; init; }
    public int ReturnedChunks { get; init; }
    public double AverageScore { get; init; }
    public double HighestScore { get; init; }
    public double LowestScore { get; init; }
    public IReadOnlyCollection<string> Sources { get; init; } = [];
}
