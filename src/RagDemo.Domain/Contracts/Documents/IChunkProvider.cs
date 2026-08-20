using RagDemo.Domain.Models;
namespace RagDemo.Domain.Contracts;

public interface IChunkProvider
{
    Task<IReadOnlyCollection<DocumentChunk>> GetChunksAsync(
        CancellationToken cancellationToken = default);
}