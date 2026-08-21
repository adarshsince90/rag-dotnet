using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;

public sealed class VectorRetriever
    : IRetriever
{
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly IRetrievalResultProcessor _resultProcessor;
    private readonly IVectorStore _vectorStore;

    public VectorRetriever(
        IEmbeddingGenerator embeddingGenerator,
        IChunkStore chunkStore,
        IRetrievalResultProcessor retrievalResultProcessor,
        IVectorStore vectorStore)
    {
        _embeddingGenerator = embeddingGenerator;
        _resultProcessor = retrievalResultProcessor;
        _vectorStore = vectorStore;
    }

    public async Task<RetrievalResponse> RetrieveAsync(string question,
        CancellationToken cancellationToken = default)
    {
        var questionEmbedding =
            await _embeddingGenerator.GenerateEmbeddingAsync(question, cancellationToken);

        var results = await _vectorStore
                .SearchAsync(
                    questionEmbedding,
                    cancellationToken);

        var processed = _resultProcessor
                .Process(results);

        return new RetrievalResponse
        {
            Results = processed.Results,
            Diagnostics = new RetrievalDiagnostics
            {
                QualifiedChunks = processed.QualifiedChunks,
                ReturnedChunks = processed.ReturnedChunks,
                TopK = processed.TopK
            }
        };
    }
}