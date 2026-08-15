using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Models;

namespace RagDemo.Infrastructure.Retrieval;

public sealed class TextFileChunkProvider : IChunkProvider
{
    private const string FilePath = "../../data/raw/company-info.txt";
    public TextFileChunkProvider()
    {
    }

    public async Task<IReadOnlyCollection<DocumentChunk>> GetChunksAsync(
        CancellationToken cancellationToken = default)
    {
        var content = await File.ReadAllTextAsync(
            FilePath,
            cancellationToken);

        return content
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => new DocumentChunk
            {
                Content = x
            })
            .ToList();
    }
}