using RagDemo.Domain.Models;

namespace RagDemo.Domain.Abstractions;

public interface IChunkProvider
{
    Task<IReadOnlyCollection<DocumentChunk>> GetChunksAsync(
        CancellationToken cancellationToken = default);
}