using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;

public sealed class CharacterChunkingStrategy : IChunkingStrategy
{
    private readonly ChunkingOptions _options;

    public CharacterChunkingStrategy(
        IOptions<ChunkingOptions> options)
    {
        _options = options.Value;
    }

    public IReadOnlyCollection<DocumentChunk> CreateChunks(
        string content,
        string source)
    {
        var chunks = new List<DocumentChunk>();

        var start = 0;
        var chunkIndex = 0;

        while (start < content.Length)
        {
            var length = Math.Min(
                _options.ChunkSize,
                content.Length - start);

            var chunkContent =
                content.Substring(start, length);

            chunks.Add(new DocumentChunk
            {
                Content = chunkContent,
                Source = source,
                ChunkIndex = chunkIndex++
            });

            start +=
                _options.ChunkSize -
                _options.ChunkOverlap;
        }

        return chunks;
    }
}