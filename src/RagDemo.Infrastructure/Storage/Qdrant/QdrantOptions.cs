namespace RagDemo.Infrastructure.Storage.Qdrant;

public sealed class QdrantOptions
{
    public const string SectionName = "Qdrant";

    public string BaseUrl { get; set; } = string.Empty;

    public string CollectionName { get; set; }
        = "ragdemo-documents";

    public int VectorSize { get; set; } = 768;
}