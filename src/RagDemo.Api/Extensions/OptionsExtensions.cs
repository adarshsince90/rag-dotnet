using RagDemo.Infrastructure.Storage.Qdrant;

public static class OptionsExtensions
{
    public static IServiceCollection AddOptionsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OllamaOptions>(
            configuration.GetSection("Ollama"));

        services.Configure<DataOptions>(
            configuration.GetSection("Data"));

        services.Configure<RetrievalOptions>(
            configuration.GetSection("Retrieval"));

        services.Configure<ChunkingOptions>(
            configuration.GetSection(ChunkingOptions.SchemaName));

        services.Configure<QdrantOptions>(
            configuration.GetSection(QdrantOptions.SectionName));


        return services;
    }
}