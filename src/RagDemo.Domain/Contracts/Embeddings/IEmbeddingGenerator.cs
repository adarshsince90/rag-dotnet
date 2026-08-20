namespace RagDemo.Domain.Contracts;

public interface IEmbeddingGenerator
{
    Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}