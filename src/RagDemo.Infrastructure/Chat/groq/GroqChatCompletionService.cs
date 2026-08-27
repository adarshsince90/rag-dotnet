using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;
using RagDemo.Infrastructure.Configuration;

public sealed class GroqChatCompletionService
    : IChatCompletionService
{
    private readonly HttpClient _httpClient;

    private readonly ProviderOptions _provider;
    private readonly ILogger<GroqChatCompletionService> _logger;

    public GroqChatCompletionService(
        HttpClient httpClient,
        ILogger<GroqChatCompletionService> logger,
        IOptions<AiOptions> options)
    {
        _httpClient = httpClient;

        var aiOptions =
            options.Value;

        _provider =
            aiOptions.Providers["groq"];
        
        _logger = logger;
    }

    public async Task<string>
        GenerateAnswerAsync(
            string prompt,
            CancellationToken cancellationToken)
    {
        var request =
            new ChatCompletionRequest
            {
                Model = _provider.ChatModel,

                Messages =
                [
                    new Message
                    {
                        Role = "user",
                        Content = prompt
                    }
                ],

                Stream = false
            };

        using var httpRequest =
            new HttpRequestMessage(
                HttpMethod.Post,
                $"{_provider.BaseUrl}/chat/completions");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _provider.ApiKey);

        httpRequest.Content =
            JsonContent.Create(request);

        using var response =
            await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

        // var content =
        //     await response.Content
        //         .ReadAsStringAsync(
        //             cancellationToken);

        var responseBody =
                await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                """
                Groq Error
                Status: {Status}
                Body: {Body}
                """,
                response.StatusCode,
                responseBody);

            throw new HttpRequestException(
                $"Groq Error ({(int)response.StatusCode}): {responseBody}");
        }


        var completion =
            JsonSerializer.Deserialize<
                ChatCompletionResponse>(
                    responseBody);

        return completion?
                   .Choices
                   .FirstOrDefault()?
                   .Message
                   .Content
               ?? string.Empty;
    }

    public async IAsyncEnumerable<string>
        GenerateAsync(
            string prompt,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
    {
        // Sprint 11B implementation:
        // Keep it simple initially.

        var answer =
            await GenerateAnswerAsync(
                prompt,
                cancellationToken);

        yield return answer;
    }

    public async IAsyncEnumerable<string>
    GenerateStreamingAsync(
        string prompt,
        [EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        var request =
            new ChatCompletionRequest
            {
                Model = _provider.ChatModel,

                Messages =
                [
                    new Message
                    {
                        Role = "user",
                        Content = prompt
                    }
                ],

                Stream = true
            };

        using var httpRequest =
            new HttpRequestMessage(
                HttpMethod.Post,
                $"{_provider.BaseUrl}/chat/completions");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _provider.ApiKey);

        httpRequest.Content =
            JsonContent.Create(request);

        using var response =
            await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content
                .ReadAsStreamAsync(
                    cancellationToken);

        using var reader =
            new StreamReader(stream);

        string? line;

        while ((line =
            await reader.ReadLineAsync()) is not null)
        {
            if (!line.StartsWith("data:"))
            {
                continue;
            }

            var data =
                line["data:".Length..]
                    .Trim();

            if (data == "[DONE]")
            {
                yield break;
            }

            ChatCompletionStreamResponse? chunk;

            try
            {
                chunk =
                    JsonSerializer.Deserialize<
                        ChatCompletionStreamResponse>(
                            data);
            }
            catch
            {
                continue;
            }

            var token =
                chunk?
                    .Choices
                    .FirstOrDefault()?
                    .Delta
                    .Content;

            if (!string.IsNullOrWhiteSpace(token))
            {
                yield return token;
            }
        }
    }
}