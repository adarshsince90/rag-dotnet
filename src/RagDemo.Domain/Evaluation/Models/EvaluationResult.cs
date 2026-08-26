namespace RagDemo.Domain.Evaluation.Models;

public sealed record EvaluationResult
{
    public int TestCaseId { get; init; }
    public string Category { get; init; }
        = string.Empty;
    public string ConversationId { get; init; }
        = string.Empty;
    public string FinalQuestion { get; init; }
        = string.Empty;
    public string ExpectedSource { get; init; } = string.Empty;
    public IReadOnlyCollection<string> RetrievedSources
    { get; init; } = [];
    public bool ExpectedSourceFound { get; init; }
    public int MatchedKeywords { get; init; }
    public int TotalKeywords { get; init; }
    public double KeywordCoverage { get; init; }
    public double HighestScore { get; init; }
    public string GeneratedAnswer { get; init; }
        = string.Empty;
    public long RetrievalMs { get; init; }
    public long GenerationMs { get; init; }
    public bool GroundingPassed { get; set; }
}