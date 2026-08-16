namespace RagDemo.Domain.Models;

public sealed class DocumentChunk
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Content { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public int ChunkIndex { get; init; }
}