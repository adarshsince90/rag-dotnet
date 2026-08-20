using Microsoft.Extensions.Options;
using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Interfaces;
using RagDemo.Infrastructure.Retrieval;

public sealed class VectorRetriever
    : IRetriever
{
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly IChunkStore _chunkStore;
    private readonly IRetrievalResultProcessor _resultProcessor;

    public VectorRetriever(
        IEmbeddingGenerator embeddingGenerator,
        IChunkStore chunkStore,
        IRetrievalResultProcessor retrievalResultProcessor)
    {
        _embeddingGenerator = embeddingGenerator;
        _chunkStore = chunkStore;
        _resultProcessor = retrievalResultProcessor;
    }

    #region old way
        // public async Task<RetrievalResponse> RetrieveAsync_old1(string question,
        //     CancellationToken cancellationToken = default)
        // {
        //     var chunks = _chunkStore.Chunks;
        //     if (!_chunkStore.Chunks.Any())
        //     {
        //         throw new InvalidOperationException(
        //             "Embeddings have not been generated. Call /embeddings/generate first.");
        //     }
    
        //     var questionEmbedding =
        //         await _embeddingGenerator
        //         .GenerateEmbeddingAsync(
        //         question,
        //         cancellationToken);
    
        //     var scoredResults = chunks
        //         .Where(chunk => chunk.Embedding is not null)
        //         .Select(chunk => new RetrievalResult
        //         {
        //             Chunk = chunk,
        //             Score = CosineSimilarity.Calculate(
        //                 questionEmbedding,
        //                 chunk.Embedding!)
        //         })
        //         .ToList();
    
        //     var orderedResults = scoredResults
        //         .OrderByDescending(x => x.Score)
        //         .ToList();
    
        //     var topResults = orderedResults
        //         .Take(_retrievalOptions.TopK)
        //         .ToList();
    
        //     var rankedResults = topResults
        //         .Select((result, index) =>
        //             new RetrievalResult
        //             {
        //                 Chunk = result.Chunk,
        //                 Score = result.Score,
        //                 Rank = index + 1
        //             })
        //         .ToList();
    
        //     return new RetrievalResponse
        //     {
        //         Results = rankedResults,
    
        //         Diagnostics = new RetrievalDiagnostics
        //         {
        //             TotalChunks = chunks.Count,
        //             QualifiedChunks = orderedResults.Count,
        //             ReturnedChunks = rankedResults.Count,
        //             TopK = _retrievalOptions.TopK
        //         }
        //     };
        // }
    #endregion

    public async Task<RetrievalResponse> RetrieveAsync(string question,
        CancellationToken cancellationToken = default)
    {
        var chunks = _chunkStore.Chunks;
        if (!_chunkStore.Chunks.Any())
        {
            throw new InvalidOperationException(
                "Embeddings have not been generated. Call /embeddings/generate first.");
        }

        var questionEmbedding =
            await _embeddingGenerator.GenerateEmbeddingAsync(question, cancellationToken);

        var results = chunks
            .Where(chunk => chunk.Embedding is not null)
            .Select(chunk => new RetrievalResult
            {
                Chunk = chunk,
                Score = CosineSimilarity.Calculate(
                    questionEmbedding,
                    chunk.Embedding!)
            });

        var processed = _resultProcessor
                .Process(results);

        return new RetrievalResponse
        {
            Results = processed.Results,
            Diagnostics = new RetrievalDiagnostics
            {
                TotalChunks = chunks.Count,
                QualifiedChunks = processed.QualifiedChunks,
                ReturnedChunks = processed.ReturnedChunks,
                TopK = processed.TopK
            }
        };
    }
}