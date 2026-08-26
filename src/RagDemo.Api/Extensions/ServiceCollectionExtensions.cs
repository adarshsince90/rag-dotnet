using RagDemo.Application.Evaluation.Services;
using RagDemo.Application.Prompts;
using RagDemo.Application.Services;
using RagDemo.Domain.Contracts;
using RagDemo.Domain.Evaluation.Contracts;
using RagDemo.Infrastructure.Conversation;
using RagDemo.Infrastructure.Documents.Extraction;
using RagDemo.Infrastructure.Embedding;
using RagDemo.Infrastructure.Storage;

namespace RagDemo.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<RetrievalService>();
        services.AddScoped<GenerateEmbeddingsService>();
        services.AddScoped<QuestionAnsweringService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        // services.AddScoped<IChunkProvider,
        //     TextFileChunkProvider>();

        services.AddScoped<IRetriever,
            VectorRetriever>();

        services.AddSingleton<IChunkStore,
            InMemoryChunkStore>();

        services.AddScoped<IRetrievalResultProcessor,
            RetrievalResultProcessor>();

        services.AddHttpClient<
            IEmbeddingGenerator,
            OllamaEmbeddingGenerator>(
                client =>
                {
                    client.Timeout =
                        TimeSpan.FromMinutes(10);
                });

        services.AddHttpClient<
            IChatCompletionService,
            OllamaChatCompletionService>(
                client =>
                {
                    client.Timeout =
                        TimeSpan.FromMinutes(10);
                });
                
        services.AddScoped<IPromptBuilder,
            RagPromptBuilder>();

        // services.AddScoped<IChunkingStrategy,
        //     ParagraphChunkingStrategy>();

        services.AddSingleton<IChunkingStrategy,
            CharacterChunkingStrategy>();

        services.AddSingleton<IDocumentExtractor, 
            PdfDocumentExtractor>();

        services.AddSingleton<IChunkProvider, 
            PdfChunkProvider>();

        services.AddSingleton<IVectorStore,
            QdrantVectorStore>();

        services.AddSingleton<IConversationMemory,
            InMemoryConversationMemory>();

        services.AddSingleton<IConversationQueryBuilder,
            ConversationQueryBuilder>();

        services.AddScoped<
            ConversationQuestionAnsweringService>();

        services.AddScoped<IEvaluationService,
            EvaluationService>();

        services.AddScoped<IEvaluationDatasetProvider,
            JsonEvaluationDatasetProvider>();

        return services;
    }
}