using RagDemo.Domain.Models;

namespace RagDemo.Domain.Contracts;

public interface IChunkStore
{
    IReadOnlyCollection<DocumentChunk> Chunks { get; }

    void Save(IReadOnlyCollection<DocumentChunk> chunks);
}