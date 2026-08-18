using RagDemo.Domain.Models;

public interface IChunkStore
{
    IReadOnlyCollection<DocumentChunk> Chunks { get; }

    void Save(IReadOnlyCollection<DocumentChunk> chunks);
}