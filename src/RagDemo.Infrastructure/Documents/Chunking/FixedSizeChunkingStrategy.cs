using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;

public sealed class FixedSizeChunkingStrategy
    : IChunkingStrategy
{
    public IReadOnlyCollection<DocumentChunk> CreateChunks(string content, string source)
    {
        throw new NotImplementedException();
    }
}