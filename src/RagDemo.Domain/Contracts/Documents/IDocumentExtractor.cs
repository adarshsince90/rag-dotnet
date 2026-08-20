public interface IDocumentExtractor
{
    Task<string> ExtractTextAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}