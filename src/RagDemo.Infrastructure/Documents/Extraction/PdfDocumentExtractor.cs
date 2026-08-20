using RagDemo.Domain.Contracts;
using UglyToad.PdfPig;

namespace RagDemo.Infrastructure.Documents.Extraction;

public sealed class PdfDocumentExtractor
    : IDocumentExtractor
{
    public Task<string> ExtractTextAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        using var document =
            PdfDocument.Open(filePath);

        var text =
            string.Join(
                Environment.NewLine,
                document.GetPages()
                    .Select(page => page.Text));

        return Task.FromResult(text);
    }
}