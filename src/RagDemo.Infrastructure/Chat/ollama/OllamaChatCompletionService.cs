using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
public sealed class OllamaChatCompletionService
    : IChatCompletionService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaChatCompletionService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GenerateAnswerAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var request = new OllamaGenerateRequest
        {
            Model = _options.ChatModel,
            Prompt = prompt,
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_options.BaseUrl}/api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<
                OllamaGenerateResponse>(
                    cancellationToken: cancellationToken);

        return result?.Response ?? string.Empty;
    }
}