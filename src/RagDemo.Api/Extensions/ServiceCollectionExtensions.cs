using RagDemo.Application.Prompts;
using RagDemo.Application.Services;
using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Interfaces;
using RagDemo.Infrastructure.Documents;
using RagDemo.Infrastructure.Embedding;

namespace RagDemo.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<AskQuestionService>();
        services.AddScoped<GenerateEmbeddingsService>();
        services.AddScoped<QuestionAnsweringService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<IChunkProvider,
            TextFileChunkProvider>();

        services.AddScoped<IRetriever,
            VectorRetriever>();

        services.AddSingleton<IChunkStore,
            InMemoryChunkStore>();

        services.AddScoped<IRetrievalResultProcessor,
            RetrievalResultProcessor>();

        services.AddHttpClient<IEmbeddingGenerator,
            OllamaEmbeddingGenerator>();

        services.AddHttpClient<IChatCompletionService,
            OllamaChatCompletionService>();
        
        services.AddScoped<IPromptBuilder,
            RagPromptBuilder>();

        return services;
    }
}