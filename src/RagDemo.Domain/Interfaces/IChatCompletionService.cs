namespace RagDemo.Domain.Interfaces;

public interface IChatCompletionService
{
    Task<string> GenerateAnswerAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}