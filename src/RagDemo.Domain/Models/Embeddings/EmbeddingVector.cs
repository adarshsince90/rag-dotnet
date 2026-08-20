namespace RagDemo.Domain.Models;
public sealed class EmbeddingVector
{
    public required float[] Values { get; init; }
}