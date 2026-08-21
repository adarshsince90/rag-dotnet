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
            "Initializing vector store...");

        var vectorStore =
            scope.ServiceProvider
                .GetRequiredService<IVectorStore>();

            await vectorStore.InitializeAsync();

        logger.LogInformation(
            "Vector store initialized.");
    }
}