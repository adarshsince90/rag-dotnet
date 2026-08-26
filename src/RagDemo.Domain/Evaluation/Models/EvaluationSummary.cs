namespace RagDemo.Domain.Evaluation.Models;

public sealed record EvaluationSummary
{
    public int TotalQuestions { get; init; }

    public int ExpectedSourceFoundCount { get; init; }

    public double ExpectedSourceCoverage { get; init; }

    public double AverageKeywordCoverage { get; init; }

    public int GroundingPasses { get; init; }

    public double GroundingAccuracy { get; init; }

    public double AverageRetrievalMs { get; init; }

    public double AverageGenerationMs { get; init; }

    public double AverageHighestScore { get; init; }

    public IReadOnlyCollection<EvaluationResult> Results { get; init; } = [];

    // public int SourceMatches { get; init; }

    // public int KeywordMatches { get; init; }

    // public double RetrievalAccuracy { get; init; }
}