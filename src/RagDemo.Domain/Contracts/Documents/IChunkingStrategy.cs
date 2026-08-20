using RagDemo.Domain.Models;

namespace RagDemo.Domain.Contracts;

public interface IChunkingStrategy
{
    IReadOnlyCollection<DocumentChunk> CreateChunks(
        string content,
        string source);
}