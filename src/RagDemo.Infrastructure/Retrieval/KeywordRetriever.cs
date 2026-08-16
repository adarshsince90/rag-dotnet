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

    public Task<IReadOnlyCollection<RetrievalResult>> RetrieveAsync_old(
        string question,
        IReadOnlyCollection<DocumentChunk> chunks,
        int topK = 3,
        CancellationToken cancellationToken = default)
    {
        var words = _wordRegex.Replace(question.ToLowerInvariant(), "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        words = words
            .Where(word => !_excludedWords.Contains(word))
            .ToArray();

        #region sprint 1: Calculate the best match for the question based on keyword occurrences
        // sprint 1: Calculate the best match for the question based on keyword occurrences
        // var bestMatch = chunks
        //         .Select(chunk => new RetrievalResult
        //         {
        //             Chunk = chunk,
        //             Score = words.Count(word =>
        //                 chunk.Content
        //                     .ToLowerInvariant()
        //                     .Contains(word))
        //         })
        //         .OrderByDescending(x => x.Score)
        //         .FirstOrDefault();

        // return Task.FromResult(bestMatch);
        #endregion

        // sprint 2: Calculate the best match for the question based on keyword occurrences and return top K matches
        var scoredResults = chunks
            .Select(chunk => new RetrievalResult
            {
                Chunk = chunk,
                Score = words.Count(word =>
                    chunk.Content
                        .ToLowerInvariant()
                        .Contains(word))
            });

        var orderedResults = scoredResults
            .OrderByDescending(x => x.Score)
            .ToList();

        if (orderedResults.Count == 0 || (orderedResults.Count != 0 && orderedResults.First().Score == 0))
        {
            return Task.FromResult((IReadOnlyCollection<RetrievalResult>)
                 new List<RetrievalResult>()
            );

            #region not recommended to return a result with score 0, as it may lead to confusion. Instead, return an empty list to indicate no relevant results were found.
            // return Task.FromResult((IReadOnlyCollection<RetrievalResult>)new List<RetrievalResult>(
            //     [new RetrievalResult { Chunk = new DocumentChunk{ Content = "No results found" }, Score = 0, Rank = 0 }]
            // ));
            #endregion
        }

        var topResults = orderedResults
            .Take(topK)
            .ToList();

        var rankedResults = topResults
            .Select((result, index) => new RetrievalResult
            {
                Chunk = result.Chunk,
                Score = result.Score,
                Rank = index + 1
            })
            .ToList();

        return Task.FromResult((IReadOnlyCollection<RetrievalResult>)rankedResults);

        #region sprint 2: Calculate the best match for the question based on keyword occurrences and return top K matches
        // sprint 2: Calculate the best match for the question based on keyword occurrences and return top K matches
        // var bestMatch = chunks
        //     .Select(chunk => new RetrievalResult
        //     {
        //         Chunk = chunk,
        //         Score = words.Count(word =>
        //             chunk.Content
        //                 .ToLowerInvariant()
        //                 .Contains(word))
        //     })
        //     .OrderByDescending(x => x.Score)
        //     .Take(topK)
        //     .Select((result, index) => new RetrievalResult
        //     {
        //         Chunk = result.Chunk,
        //         Score = result.Score,
        //         Rank = index + 1
        //     })
        //     .ToList();

        // return Task.FromResult((IReadOnlyCollection<RetrievalResult>)bestMatch);
        #endregion
    }

    public Task<RetrievalResponse> RetrieveAsync(string question,
        IReadOnlyCollection<DocumentChunk> chunks,
        int topK = 3,
        CancellationToken cancellationToken = default)
    {
        var words = _wordRegex.Replace(question.ToLowerInvariant(), "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        words = words
            .Where(word => !_excludedWords.Contains(word))
            .ToArray();

        var scoredResults = chunks
            .Select(chunk => new RetrievalResult
            {
                Chunk = chunk,
                Score = words.Count(word =>
                    chunk.Content
                        .ToLowerInvariant()
                        .Contains(word))
            });

        var orderedResults = scoredResults
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ToList();

        // If no results are found or the top result has a score of 0, return an empty response with diagnostics
        if (orderedResults.Count == 0 || (orderedResults.Count != 0 && orderedResults.First().Score == 0))
        {
            return Task.FromResult(new RetrievalResponse
            {
                Results = new List<RetrievalResult>(),
                Diagnostics = new RetrievalDiagnostics
                {
                    TotalChunks = chunks.Count,
                    QualifiedChunks = 0,
                    ReturnedChunks = 0,
                    TopK = topK
                }
            });
        }

        var topResults = orderedResults
            .Take(topK)
            .ToList();

        var rankedResults = topResults
            .Select((result, index) => new RetrievalResult
            {
                Chunk = result.Chunk,
                Score = result.Score,
                Rank = index + 1
            })
            .ToList();

        return Task.FromResult(new RetrievalResponse
        {
            Results = rankedResults,
            Diagnostics = new RetrievalDiagnostics
            {
                TotalChunks = chunks.Count,
                QualifiedChunks = orderedResults.Count(x => x.Score > 0),
                ReturnedChunks = rankedResults.Count,
                TopK = topK
            }
        });
    }
}