namespace RagDemo.Infrastructure.Configuration;

public sealed class RetrievalOptions
{
    public int TopK { get; init; } = 3;
    public double MinimumSimilarity { get; init; } = 0.60;
    public int SearchLimit { get; set; } = 50;
}