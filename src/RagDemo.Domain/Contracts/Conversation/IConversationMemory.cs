namespace RagDemo.Domain.Contracts;

public interface IConversationMemory
{
    Task AddTurnAsync(
        string conversationId,
        ConversationTurn turn,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ConversationTurn>>
        GetRecentTurnsAsync(
            string conversationId,
            CancellationToken cancellationToken = default);

    Task ClearConversationAsync(
        string conversationId,
        CancellationToken cancellationToken = default);
}