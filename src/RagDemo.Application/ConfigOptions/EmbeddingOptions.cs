public sealed class EmbeddingOptions
{
    public const string SchemaName = "Embeddings";
    public int MaxConcurrency { get; init; } = 8;
}