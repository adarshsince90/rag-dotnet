using RagDemo.Domain.Contracts;

namespace RagDemo.Infrastructure.Conversation;

public sealed class ConversationQueryBuilder
    : IConversationQueryBuilder
{
    public string BuildQuery(
        string currentQuestion,
        IReadOnlyCollection<ConversationTurn> history)
    {
        if (!history.Any())
        {
            return currentQuestion;
        }

        var questions =
            history
                .Select(x => x.Question)
                .Append(currentQuestion);

        return string.Join(
            Environment.NewLine,
            questions);
    }
}