using RagDemo.Domain.Models;

public interface IVectorStore
{
    Task InitializeAsync(
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        IReadOnlyCollection<DocumentChunk> chunks,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RetrievalResult>> SearchAsync(
        float[] embedding,
        CancellationToken cancellationToken = default);
}