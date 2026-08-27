public sealed class RetrievalMetrics
{
    public long RetrievalMs { get; set; }

    public int ReturnedChunks { get; set; }

    public double AverageScore { get; set; }

    public double HighestScore { get; set; }

    public double LowestScore { get; set; }

    public List<string> Sources { get; set; }
        = [];
}