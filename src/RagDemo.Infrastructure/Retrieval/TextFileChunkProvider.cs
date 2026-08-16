using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Models;

namespace RagDemo.Infrastructure.Retrieval;

public sealed class TextFileChunkProvider : IChunkProvider
{
    // todo: make this configurable
    // private const string FilePath = "../../data/raw/company-info.txt";
    private const string FolderPath = "../../data/raw/";
    public TextFileChunkProvider()
    {
    }

    public async Task<IReadOnlyCollection<DocumentChunk>> GetChunksAsync(
        CancellationToken cancellationToken = default)
    {
       #region sprint 1: Read the text file and split it into chunks based on sentences
         // sprint 1: Read the text file and split it into chunks based on sentences
         // var content = await File.ReadAllTextAsync(
         //     FilePath,
         //     cancellationToken);
 
         // return content
         //     .Split('.', StringSplitOptions.RemoveEmptyEntries)
         //     .Select(x => x.Trim())
         //     .Where(x => !string.IsNullOrWhiteSpace(x))
         //     .Select((content, index) => new DocumentChunk
         //     {
         //         Content = content,
         //         Source = Path.GetFileName(FilePath),
         //         ChunkIndex = index
         //     })
         //     .ToList();
       #endregion

        // Get all text files in the specified folder
         var files = Directory.GetFiles(
                    FolderPath,
                    "*.txt");

        List<DocumentChunk> result = new List<DocumentChunk>();
        
        foreach(var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            var chunks = CreateChunk(text, file);
            result.AddRange(chunks);
        }
        return result;
    }

    private static List<DocumentChunk> CreateChunk(string content, string filepath)
    {
         return content
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select((content, index) => new DocumentChunk
            {
                Content = content,
                Source = Path.GetFileName(filepath),
                ChunkIndex = index
            })
            .ToList();
    }
}