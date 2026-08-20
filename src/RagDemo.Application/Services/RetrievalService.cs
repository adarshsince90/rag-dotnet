using RagDemo.Application.Dtos;
using RagDemo.Domain.Abstractions;

namespace RagDemo.Application.Services;

public sealed class RetrievalService
{
    private readonly IRetriever _retriever;

    public RetrievalService(
        IRetriever retriever)
    {
        _retriever = retriever;
    }

    public async Task<RetrieveResponse> RetrieveAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        // var chunks = await _chunkProvider
        //     .GetChunksAsync(cancellationToken);

        // var match = await _retriever
        //     .RetrieveAsync(
        //         question,
        //         chunks,
        //         cancellationToken);

        // return new RetrieveResponse(
        //     question,
        //     match?.Chunk?.Content,
        //     match?.Score ?? 0);
        // ---------------------------------------

        // var matches = await _retriever
        //     .RetrieveAsync(
        //         question,
        //         chunks,
        //         topK: 3,
        //         cancellationToken);
        
        var retrievalResponse =
            await _retriever.RetrieveAsync(
                question,
                cancellationToken: cancellationToken);
        
        var matches = retrievalResponse.Results
            .Select(result =>
                new MatchResponse(
                    result.Rank,
                    result.Score,
                    result.Chunk.Content,
                    result.Chunk.Source))
            .ToList();

        var diagnostics =
            new RetrievalDiagnosticsResponse(
                retrievalResponse.Diagnostics.TotalChunks,
                retrievalResponse.Diagnostics.QualifiedChunks,
                retrievalResponse.Diagnostics.ReturnedChunks,
                retrievalResponse.Diagnostics.TopK);

        return new RetrieveResponse(
            question,
            diagnostics,
            matches);
    }
}