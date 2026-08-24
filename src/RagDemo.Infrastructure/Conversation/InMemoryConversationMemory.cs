using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Contracts;

namespace RagDemo.Infrastructure.Conversation;

public sealed class InMemoryConversationMemory
    : IConversationMemory
{
    private readonly ConcurrentDictionary<
        string,
        List<ConversationTurn>> _conversations =
            new();

    private readonly ConversationOptions _options;

    public InMemoryConversationMemory(
        IOptions<ConversationOptions> options)
    {
        _options = options.Value;
    }

    public Task AddTurnAsync(
        string conversationId,
        ConversationTurn turn,
        CancellationToken cancellationToken = default)
    {
        var turns =
            _conversations.GetOrAdd(
                conversationId,
                _ => []);

        lock (turns)
        {
            turns.Add(turn);

            if (turns.Count > _options.MaxHistoryTurns)
            {
                turns.RemoveAt(0);
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<ConversationTurn>>
        GetRecentTurnsAsync(
            string conversationId,
            CancellationToken cancellationToken = default)
    {
        if (!_conversations.TryGetValue(
                conversationId,
                out var turns))
        {
            return Task.FromResult<
                IReadOnlyCollection<ConversationTurn>>([]);
        }

        lock (turns)
        {
            return Task.FromResult<
                IReadOnlyCollection<ConversationTurn>>(
                turns
                    .OrderBy(x => x.Timestamp)
                    .TakeLast(_options.MaxHistoryTurns)
                    .ToList());
        }
    }

    public Task ClearConversationAsync(
        string conversationId,
        CancellationToken cancellationToken = default)
    {
        _conversations.TryRemove(
            conversationId,
            out _);

        return Task.CompletedTask;
    }
}