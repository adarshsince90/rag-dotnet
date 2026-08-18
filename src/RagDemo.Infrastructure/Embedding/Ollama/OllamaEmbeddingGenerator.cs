using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using RagDemo.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace RagDemo.Infrastructure.Embedding;

public sealed class OllamaEmbeddingGenerator : IEmbeddingGenerator
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaEmbeddingGenerator(HttpClient httpClient, IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model = _options.EmbeddingModel,
            prompt = text
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/api/embeddings")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var embeddingResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: cancellationToken);
        return embeddingResponse?.Embedding ?? [];
    }
}