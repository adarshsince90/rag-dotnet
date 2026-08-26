using System.Text.RegularExpressions;
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

        // var text =
        //     string.Join(
        //         Environment.NewLine,
        //         document.GetPages()
        //             .Select(page => page.Text));

         var pages = document
            .GetPages()
            .Select(page =>
                string.Join(
                    " ",
                    page.GetWords()
                        .Select(word => word.Text)));

        var text =
            string.Join(
                Environment.NewLine,
                pages);
        
        Regex.Replace(text, @"\s+", " ");
        text = text.Replace(" .", ".");
        text = text.Replace(" ,", ",");

        return Task.FromResult(text);
    }
}