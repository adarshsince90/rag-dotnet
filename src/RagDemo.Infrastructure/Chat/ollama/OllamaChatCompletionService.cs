using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Infrastructure.Configuration;
public sealed class OllamaChatCompletionService
    : IChatCompletionService
{
    private readonly HttpClient _httpClient;
    private readonly ProviderOptions _provider;
    private readonly ILogger<OllamaChatCompletionService> _logger;

    public OllamaChatCompletionService(
        HttpClient httpClient,
        IOptions<AiOptions> options,
        ILogger<OllamaChatCompletionService> logger)
    {
        _httpClient = httpClient;
        
        var aiOptions = options.Value;
        _provider =
            aiOptions.Providers[
            aiOptions.DefaultProvider];

        _logger = logger;
    }

    public async Task<string> GenerateAnswerAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var request = new OllamaGenerateRequest
        {
            Model = _provider.ChatModel,
            Prompt = prompt,
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_provider.BaseUrl}/api/generate",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            _logger.LogError("Ollama Error: {Error}", error);
        }

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<
                OllamaGenerateResponse>(
                    cancellationToken: cancellationToken);

        return result?.Response ?? string.Empty;
    }

    public async IAsyncEnumerable<string> GenerateStreamingAsync(
    string prompt,
    [EnumeratorCancellation]
    CancellationToken cancellationToken = default)
    {
        var request = new OllamaGenerateRequest
        {
            Model = _provider.ChatModel,
            Prompt = prompt,
            Stream = true
        };

        
        _logger.LogInformation("Calling ollama /generate api with {request}... ",request.Prompt.ToString());

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_provider.BaseUrl}/api/generate")
        {
            Content = JsonContent.Create(request)
        };

        var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadAsStringAsync();

            _logger.LogError(
                "Ollama Error: {Error}",
                error);
        }

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var reader = new StreamReader(stream);

        string? line;

        while ((line = await reader.ReadLineAsync(
            cancellationToken)) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk =
                JsonSerializer.Deserialize<
                    OllamaGenerateResponse>(line);

            // _logger.LogDebug("Raw Ollama response: {Line}",line);
            
            // _logger.LogDebug("Response='{Response}', Done={Done}",chunk?.Response,chunk?.Done);

            if (!string.IsNullOrWhiteSpace(
                    chunk?.Response))
            {
                yield return chunk.Response;
            }
            
            if (chunk?.Done == true)
                yield break;
        }
        yield break;
    }
}