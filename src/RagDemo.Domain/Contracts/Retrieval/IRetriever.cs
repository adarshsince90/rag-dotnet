using RagDemo.Domain.Models;

namespace RagDemo.Domain.Contracts;

public interface IRetriever
{
    // Task<RetrievalResult?> RetrieveAsync(
    //     string question,
    //     IReadOnlyCollection<DocumentChunk> chunks,
    //     CancellationToken cancellationToken = default);

    // Task<IReadOnlyCollection<RetrievalResult>>
    //     RetrieveAsync(
    //     string question,
    //     IReadOnlyCollection<DocumentChunk> chunks,
    //     int topK = 3,
    //     CancellationToken cancellationToken = default);

    // Task<RetrievalResponse> RetrieveAsync(
    //     string question,
    //     IReadOnlyCollection<DocumentChunk> chunks,
    //     int topK = 3,
    //     CancellationToken cancellationToken = default);

    Task<RetrievalResponse> RetrieveAsync(
        string question,
        CancellationToken cancellationToken = default);
}