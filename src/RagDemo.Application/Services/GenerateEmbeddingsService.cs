using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Interfaces;
using RagDemo.Domain.Models;

namespace RagDemo.Application.Services;

public sealed class GenerateEmbeddingsService
{
    private readonly IChunkProvider _chunkProvider;
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly IChunkStore _chunkStore;

    public GenerateEmbeddingsService(
        IChunkProvider chunkProvider,
        IEmbeddingGenerator embeddingGenerator,
        IChunkStore chunkStore)
    {
        _chunkProvider = chunkProvider;
        _embeddingGenerator = embeddingGenerator;
        _chunkStore = chunkStore;
    }

    public async Task<IReadOnlyCollection<DocumentChunk>> GenerateEmbeddingsAsync(
        CancellationToken cancellationToken = default)
    {
     #region Sprint 3
           // var chunks = await _chunkProvider
           //     .GetChunksAsync(cancellationToken);
   
           // foreach (var chunk in chunks)
           // {
           //     var embedding = await _embeddingGenerator
           //         .GenerateEmbeddingAsync(
           //             chunk.Content,
           //             cancellationToken);
   
           //     chunk.Embedding = embedding;
   
           //     Console.WriteLine($"Chunk {chunk.ChunkIndex}: {embedding.Length} dimensions");
           // }
     #endregion

        // Save the chunks with embeddings to the store
        // todo: use bacthes in future to avoid memory issues with large documents
        var chunks = (await _chunkProvider
            .GetChunksAsync(cancellationToken))
            .ToList();

        var embeddings = await Task.WhenAll(
            chunks.Select(chunk =>
                _embeddingGenerator.GenerateEmbeddingAsync(
                    chunk.Content,
                    cancellationToken)));

        for (var i = 0; i < chunks.Count; i++)
        {
            chunks[i].Embedding = embeddings[i];
        }

        _chunkStore.Save(chunks);
        
        return chunks;
    }
}