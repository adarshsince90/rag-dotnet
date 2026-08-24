namespace RagDemo.Domain.Contracts;

public interface IChatCompletionService
{
    Task<string> GenerateAnswerAsync(
        string prompt,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> GenerateStreamingAsync(
        string prompt,
        CancellationToken cancellationToken);
}