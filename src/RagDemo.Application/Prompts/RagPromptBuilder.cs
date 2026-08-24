using System.Text;
using RagDemo.Domain.Contracts;

namespace RagDemo.Application.Prompts;

public sealed class RagPromptBuilder : IPromptBuilder
{
    public string BuildPrompt(
        string question,
        string context)
    {
        return $"""
You are a helpful assistant.

Answer ONLY using the provided context.

If the answer cannot be found in the context,
reply with:

"I could not find the answer in the provided documents."

Context:
{context}

Question:
{question}
""";
    }

    public string BuildConversationPrompt(
        string question,
        string context,
        IReadOnlyCollection<ConversationTurn> history)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            "You are a helpful assistant.");

        sb.AppendLine();

        sb.AppendLine(
            "Answer ONLY using the provided context.");

        sb.AppendLine();

        if (history.Any())
        {
            sb.AppendLine(
                "Conversation History:");

            foreach (var turn in history)
            {
                sb.AppendLine(
                    $"User: {turn.Question}");

                sb.AppendLine(
                    $"Assistant: {turn.Answer}");

                sb.AppendLine();
            }
        }

        sb.AppendLine(
            "Context:");

        sb.AppendLine(context);

        sb.AppendLine();

        sb.AppendLine(
            $"Question: {question}");

        return sb.ToString();
    }
}