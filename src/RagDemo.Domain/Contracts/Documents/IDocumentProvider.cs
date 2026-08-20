using RagDemo.Domain.Models;

public interface IDocumentProvider
{
    Task<IReadOnlyCollection<DocumentChunk>>
        GetChunksAsync(
            CancellationToken cancellationToken = default);
}