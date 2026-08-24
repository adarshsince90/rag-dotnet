public sealed record RetrievalMetrics
{
    public int ReturnedChunks { get; init; }
    public double AverageScore { get; init; }
    public double HighestScore { get; init; }
    public double LowestScore { get; init; }
}
