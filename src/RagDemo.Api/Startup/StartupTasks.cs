using RagDemo.Application.Services;

public static class StartupTasks
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var logger =
            scope.ServiceProvider
                .GetRequiredService<
                    ILoggerFactory>()
                .CreateLogger("Startup");

        logger.LogInformation(
            "Generating embeddings...");

        var embeddingService =
            scope.ServiceProvider
                .GetRequiredService<GenerateEmbeddingsService>();

        var chunks =
            await embeddingService.GenerateEmbeddingsAsync();

        logger.LogInformation(
            "Generated embeddings for {ChunkCount} chunks.",
            chunks.Count);
    }
}