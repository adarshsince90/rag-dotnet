using RagDemo.Domain.Models;

public sealed class RetrievalResult
{
    public DocumentChunk? Chunk { get; init; }

    public double Score { get; init; }
}