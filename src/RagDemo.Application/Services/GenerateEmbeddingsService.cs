using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;

namespace RagDemo.Application.Services;

public sealed class GenerateEmbeddingsService
{
    private readonly IChunkProvider _chunkProvider;
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly IChunkStore _chunkStore;
    private readonly IVectorStore _vectorStore;
    private readonly ILogger<GenerateEmbeddingsService> _logger;
    private readonly EmbeddingOptions _embeddingOptions;

    public GenerateEmbeddingsService(
        IChunkProvider chunkProvider,
        IEmbeddingGenerator embeddingGenerator,
        IChunkStore chunkStore,
        ILogger<GenerateEmbeddingsService> logger,
        IVectorStore vectorStore,
        IOptions<EmbeddingOptions> embeddingOptions)
    {
        _chunkProvider = chunkProvider;
        _embeddingGenerator = embeddingGenerator;
        _chunkStore = chunkStore;
        _vectorStore = vectorStore;
        _logger = logger;
        _embeddingOptions = embeddingOptions.Value;
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

     #region Sprint 5
           // // Save the chunks with embeddings to the store
           // // todo: use bacthes in future to avoid memory issues with large documents
           // var chunks = (await _chunkProvider
           //     .GetChunksAsync(cancellationToken))
           //     .ToList();
   
           // _logger.LogInformation(
           //     "Generating embeddings for {ChunkCount} chunks.",
           //     chunks.Count);
   
           // var stopwatch = Stopwatch.StartNew();
   
           // var semaphore = new SemaphoreSlim(8);
   
           // var tasks = chunks.Select(async (chunk, index) =>
           // {
           //     await semaphore.WaitAsync(cancellationToken);
   
           //     try
           //     {
           //         _logger.LogInformation(
           //             "Starting chunk {ChunkIndex}",
           //             index);
   
           //         var embedding = await _embeddingGenerator
           //             .GenerateEmbeddingAsync(
           //                 chunk.Content,
           //                 cancellationToken);
   
           //         _logger.LogInformation(
           //             "Completed chunk {ChunkIndex}",
           //             index);
                   
           //         return embedding;
           //     }
           //     finally
           //     {
           //         semaphore.Release();
           //     }
           // });
   
           // var embeddings = await Task.WhenAll(tasks);
   
           // // var embeddings = await Task.WhenAll(
           // //     chunks.Select(async (chunk, index) =>
           // //     {
           // //         _logger.LogDebug(
           // //             "Starting chunk {ChunkIndex}",
           // //             index);
   
           // //         var embedding =
           // //             await _embeddingGenerator
           // //                 .GenerateEmbeddingAsync(
           // //                     chunk.Content,
           // //                     cancellationToken);
   
           // //         _logger.LogDebug(
           // //             "Completed chunk {ChunkIndex}",
           // //             index);
   
           // //         return embedding;
           // //     }));
   
           // stopwatch.Stop();
   
           // _logger.LogInformation(
           //         "Generated embeddings in {DurationMs}ms",
           //         stopwatch.ElapsedMilliseconds);
   
           // for (var i = 0; i < chunks.Count; i++)
           // {
           //     chunks[i].Embedding = embeddings[i];
           // }
   
           // _logger.LogInformation(
           //     "Embedding generation completed.");
   
           // _chunkStore.Save(chunks);
           
           // return chunks;
     #endregion

        // Sprint 6 Save the chunks with embeddings to the Qdrant vector DB store
        var chunks = (await _chunkProvider
            .GetChunksAsync(cancellationToken))
            .ToList();

        _logger.LogInformation(
            "Generating embeddings for {ChunkCount} chunks.",
            chunks.Count);

        var stopwatch = Stopwatch.StartNew();

        var semaphore = new SemaphoreSlim(_embeddingOptions.MaxConcurrency);

        var tasks = chunks.Select(async (chunk, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                _logger.LogInformation(
                    "Starting chunk {ChunkIndex}",
                    index);

                var embedding = await _embeddingGenerator
                    .GenerateEmbeddingAsync(
                        chunk.Content,
                        cancellationToken);

                _logger.LogInformation(
                    "Completed chunk {ChunkIndex}",
                    index);
                
                return embedding;
            }
            finally
            {
                semaphore.Release();
            }
        });

        var embeddings = await Task.WhenAll(tasks);

        stopwatch.Stop();

        _logger.LogInformation(
            "Embedding dimension: {Dimension}",
            chunks.First().Embedding?.Length);
            
        _logger.LogInformation(
                "Generated embeddings in {DurationMs}ms",
                stopwatch.ElapsedMilliseconds);

        for (var i = 0; i < chunks.Count; i++)
        {
            chunks[i].Embedding = embeddings[i];
        }

        _logger.LogInformation(
            "Embedding generation completed.");

        _chunkStore.Save(chunks);

        await _vectorStore.UpsertAsync(
            chunks,
            cancellationToken);
        
        return chunks;
    }
}