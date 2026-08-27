using System.Text.Json.Serialization;

public sealed class ChatCompletionStreamResponse
{
    [JsonPropertyName("choices")]
    public List<StreamChoice> Choices
    {
        get;
        set;
    } = [];
}

public sealed class StreamChoice
{
    [JsonPropertyName("delta")]
    public StreamDelta Delta
    {
        get;
        set;
    } = new();
}

public sealed class StreamDelta
{
    [JsonPropertyName("content")]
    public string? Content
    {
        get;
        set;
    }
}
