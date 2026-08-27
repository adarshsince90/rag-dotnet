using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;
using RagDemo.Infrastructure.Configuration;

public sealed class PdfChunkProvider : IChunkProvider
{
    private readonly IDocumentExtractor _extractor;
    private readonly IChunkingStrategy _chunkingStrategy;
    private readonly DataOptions _dataOptions;

    public PdfChunkProvider(IDocumentExtractor documentExtractor, IChunkingStrategy chunkingStrategy, IOptions<DataOptions> options)
    {
        _extractor = documentExtractor;
        _chunkingStrategy = chunkingStrategy;
        _dataOptions = options.Value;
    }

    public async Task<IReadOnlyCollection<DocumentChunk>> GetChunksAsync(CancellationToken cancellationToken = default)
    {
        var pdfFiles = Directory.GetFiles(_dataOptions.PdfDirectory,"*.pdf", SearchOption.TopDirectoryOnly);
        var allChunks = new List<DocumentChunk>();

        foreach (var pdf in pdfFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var text =
                await _extractor.ExtractTextAsync(pdf,cancellationToken);

            var chunks =
                _chunkingStrategy.CreateChunks(
                    text,
                    Path.GetFileName(pdf));

            allChunks.AddRange(chunks);
        }

        return allChunks.AsReadOnly<DocumentChunk>();
    }
}