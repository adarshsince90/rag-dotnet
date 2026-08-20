using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Models;

public sealed class OverlapChunkingStrategy
    : IChunkingStrategy
{
    public IReadOnlyCollection<DocumentChunk> CreateChunks(string content, string source)
    {
        throw new NotImplementedException();
    }
}