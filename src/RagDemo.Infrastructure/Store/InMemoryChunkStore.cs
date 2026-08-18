using RagDemo.Domain.Models;

public sealed class InMemoryChunkStore : IChunkStore
{
    private IReadOnlyCollection<DocumentChunk> _chunks = [];

    public IReadOnlyCollection<DocumentChunk> Chunks
        => _chunks;

    public void Save(
        IReadOnlyCollection<DocumentChunk> chunks)
    {
        _chunks = chunks;
    }
}