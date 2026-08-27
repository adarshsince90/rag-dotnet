using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Infrastructure.Configuration;

namespace RagDemo.Infrastructure.Embedding;

public sealed class OllamaEmbeddingGenerator : IEmbeddingGenerator
{
    private readonly HttpClient _httpClient;
    private readonly ProviderOptions _provider;

    public OllamaEmbeddingGenerator(HttpClient httpClient, 
        IOptions<AiOptions> options)
    {
        _httpClient = httpClient;

        var aiOptions = options.Value;
        _provider =
            aiOptions.Providers[
            "local"];
    }

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model = _provider.EmbeddingModel,
            prompt = text
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_provider.BaseUrl}/api/embeddings")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var embeddingResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: cancellationToken);
        return embeddingResponse?.Embedding ?? [];
    }
}