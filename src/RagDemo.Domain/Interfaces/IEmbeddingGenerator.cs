namespace RagDemo.Domain.Interfaces;

public interface IEmbeddingGenerator
{
    Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}