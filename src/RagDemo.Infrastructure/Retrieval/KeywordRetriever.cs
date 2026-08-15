using System.Text.RegularExpressions;
using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Models;

namespace RagDemo.Infrastructure.Retrieval;

public sealed class KeywordRetriever : IRetriever
{
    private readonly Regex _wordRegex = new Regex(@"[^\w\s]", RegexOptions.Compiled);
    private readonly HashSet<string> _excludedWords = new HashSet<string>
    {
        "the", "is", "in", "at", "of", "and", "a", "to", "where", "what", "when", "the", "is", "are"
    };

    public Task<RetrievalResult?> RetrieveAsync(
        string question,
        IReadOnlyCollection<DocumentChunk> chunks,
        CancellationToken cancellationToken = default)
    {
        var words = _wordRegex.Replace(question.ToLowerInvariant(), "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        words = words.Where(word => !_excludedWords.Contains(word)).ToArray();

        var bestMatch = chunks
                .Select(chunk => new RetrievalResult
                {
                    Chunk = chunk,
                    Score = words.Count(word =>
                        chunk.Content
                            .ToLowerInvariant()
                            .Contains(word))
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

        // var result = chunks
        //     .OrderByDescending(chunk =>
        //         words.Count(word =>
        //             chunk.Content
        //                 .ToLowerInvariant()
        //                 .Contains(word)))
        //     .FirstOrDefault();

        // var score = result != null
        //     ? words.Count(word => result.Content.ToLowerInvariant().Contains(word))
        //     : 0;

        return Task.FromResult(bestMatch);
    }
}