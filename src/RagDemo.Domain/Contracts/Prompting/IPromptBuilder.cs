namespace RagDemo.Domain.Contracts;

public interface IPromptBuilder
{
    string BuildPrompt(
        string question,
        string context);

    string BuildConversationPrompt(
        string question,
        string context,
        IReadOnlyCollection<ConversationTurn> history
    );
}