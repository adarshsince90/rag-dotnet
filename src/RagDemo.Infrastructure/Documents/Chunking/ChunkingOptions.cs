public sealed class ChunkingOptions
{
    public int ChunkSize { get; init; } = 1000;

    public int ChunkOverlap { get; init; } = 200;
}