using RagDemo.Application.Dtos;
using RagDemo.Domain.Abstractions;

namespace RagDemo.Application.Services;

public sealed class AskQuestionService
{
    private readonly IChunkProvider _chunkProvider;
    private readonly IRetriever _retriever;

    public AskQuestionService(
        IChunkProvider chunkProvider,
        IRetriever retriever)
    {
        _chunkProvider = chunkProvider;
        _retriever = retriever;
    }

    public async Task<AskQuestionResponse> AskAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        var chunks = await _chunkProvider
            .GetChunksAsync(cancellationToken);

        var match = await _retriever
            .RetrieveAsync(
                question,
                chunks,
                cancellationToken);

        return new AskQuestionResponse(
            question,
            match?.Chunk?.Content,
            match?.Score ?? 0);
    }
}