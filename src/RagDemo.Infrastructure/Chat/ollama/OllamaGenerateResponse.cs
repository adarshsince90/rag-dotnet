using System.Text.Json.Serialization;

public sealed class OllamaGenerateResponse
{
    [JsonPropertyName("response")]
    public string Response { get; init; } = string.Empty;
    [JsonPropertyName("done")]
    public bool Done { get; set; }
}