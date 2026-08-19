public sealed class OllamaGenerateRequest
{
    public required string Model { get; init; }

    public required string Prompt { get; init; }

    public bool Stream { get; init; } = false;
}