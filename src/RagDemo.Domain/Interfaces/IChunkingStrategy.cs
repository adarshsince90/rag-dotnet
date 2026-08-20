using RagDemo.Domain.Models;

namespace RagDemo.Domain.Abstractions;

public interface IChunkingStrategy
{
    IReadOnlyCollection<DocumentChunk> CreateChunks(
        string content,
        string source);
}