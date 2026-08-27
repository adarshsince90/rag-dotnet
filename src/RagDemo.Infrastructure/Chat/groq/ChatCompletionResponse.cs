using System.Text.Json.Serialization;

public sealed class ChatCompletionResponse
{
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; } = [];
}

public sealed class Choice
{
    [JsonPropertyName("message")]
    public Message Message { get; set; } = new();
}