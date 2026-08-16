using RagDemo.Domain.Models;

public sealed class RetrievalResult
{
    public required DocumentChunk Chunk { get; init; }

    public double Score { get; init; }

    public int Rank { get; init; }
}