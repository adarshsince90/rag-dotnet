using RagDemo.Domain.Models;

namespace RagDemo.Domain.Abstractions;

public interface IRetriever
{
    Task<RetrievalResult?> RetrieveAsync(
        string question,
        IReadOnlyCollection<DocumentChunk> chunks,
        CancellationToken cancellationToken = default);
}