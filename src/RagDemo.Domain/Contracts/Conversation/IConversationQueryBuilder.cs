namespace RagDemo.Domain.Contracts;
public interface IConversationQueryBuilder
{
    string BuildQuery(
        string currentQuestion,
        IReadOnlyCollection<ConversationTurn> history);
}