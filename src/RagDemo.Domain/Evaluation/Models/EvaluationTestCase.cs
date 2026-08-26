using System.Text.Json.Serialization;

namespace RagDemo.Domain.Evaluation.Models;

public sealed record EvaluationTestCase
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("question")]
    public string Question { get; init; } = string.Empty;

    [JsonPropertyName("conversationId")]
    public string ConversationId { get; init; } = string.Empty;

    [JsonPropertyName("history")]
    public IReadOnlyCollection<string> History { get; init; } = [];

    [JsonPropertyName("expectedSource")]
    public string ExpectedSource { get; init; } = string.Empty;

    [JsonPropertyName("expectedKeywords")]
    public IReadOnlyCollection<string> ExpectedKeywords { get; init; }
        = [];

    [JsonPropertyName("expectGroundedRefusal")]
    public bool ExpectGroundedRefusal { get; init; } = false;
}