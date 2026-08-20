using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;

public sealed class ParagraphChunkingStrategy
    : IChunkingStrategy
{
    public IReadOnlyCollection<DocumentChunk> CreateChunks(
        string content,
        string source)
    {
        var chunks = new List<DocumentChunk>();

        var paragraphs = content
            .Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries);

        foreach (
            var (paragraph, index)
            in paragraphs.Select(
                (p, i) => (p, i)))
        {
            chunks.Add(
                new DocumentChunk
                {
                    Content = paragraph.Trim(),
                    Source = source,
                    ChunkIndex = index
                });
        }

        return chunks;
    }
}