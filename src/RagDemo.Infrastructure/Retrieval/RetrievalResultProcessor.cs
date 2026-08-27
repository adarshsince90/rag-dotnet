using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Domain.Models;
using RagDemo.Infrastructure.Configuration;
public sealed class RetrievalResultProcessor
    : IRetrievalResultProcessor
{
    private readonly RetrievalOptions _options;

    public RetrievalResultProcessor(
        IOptions<RetrievalOptions> options)
    {
        _options = options.Value;
    }

    public RetrievalProcessingResult Process(
        IEnumerable<RetrievalResult> results)
    {
        var filteredResults = results
            .Where(x => x.Score >= _options.MinimumSimilarity)
            .OrderByDescending(x => x.Score)
            .Take(_options.TopK)
            .ToList();

        var rankedResults = filteredResults
            .Select((result, index) => new RetrievalResult
            {
                Chunk = result.Chunk,
                Score = result.Score,
                Rank = index + 1
            })
            .ToList();

        return new RetrievalProcessingResult
        {
            Results = rankedResults,
            QualifiedChunks = results.Count(x => x.Score >= _options.MinimumSimilarity),
            ReturnedChunks = rankedResults.Count,
            TopK = _options.TopK
        };
    }
}