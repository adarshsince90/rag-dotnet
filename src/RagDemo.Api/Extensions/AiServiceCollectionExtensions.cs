using RagDemo.Domain.Contracts;
using RagDemo.Infrastructure.Configuration;

namespace RagDemo.Api.Extensions;

public static class AiServiceCollectionExtensions
{
    public static IServiceCollection AddAiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiOptions>(
            configuration.GetSection("Ai"));

        var aiOptions =
            configuration
                .GetSection("Ai")
                .Get<AiOptions>()
            ?? throw new InvalidOperationException(
                "AI configuration is missing.");

        var provider =
            aiOptions.Providers[
                aiOptions.DefaultProvider];

        switch (provider.Type)
        {
            case "Ollama":

                services.AddHttpClient<
                    OllamaChatCompletionService>(
                    client =>
                    {
                        client.Timeout =
                            TimeSpan.FromMinutes(10);
                    });

                services.AddScoped<
                    IChatCompletionService,
                    OllamaChatCompletionService>();

                break;

            case "groq":

                services.AddHttpClient<
                    GroqChatCompletionService>(
                    client =>
                    {
                        client.Timeout =
                            TimeSpan.FromMinutes(10);
                    });

                services.AddScoped<
                    IChatCompletionService,
                    GroqChatCompletionService>();

                break;

            default:

                throw new InvalidOperationException(
                    $"Unsupported AI provider: {provider.Type}");
        }

        return services;
    }
}