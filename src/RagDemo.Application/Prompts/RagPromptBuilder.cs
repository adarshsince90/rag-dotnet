using System.Text;
using RagDemo.Application.Constants;
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
            "Conversation history is provided only to help understand references and follow-up questions.");

        sb.AppendLine(
            "Do NOT use conversation history as factual knowledge.");

        sb.AppendLine();

        sb.AppendLine(
            "Use ONLY information from the retrieved document context when answering.");

        sb.AppendLine();

        sb.AppendLine(
            "You may combine, summarize and reason across multiple parts of the retrieved document context.");

        sb.AppendLine();

        sb.AppendLine(
            "Do not use prior knowledge.");

        sb.AppendLine();

        sb.AppendLine(
            "If the answer cannot reasonably be determined from the retrieved document context, reply EXACTLY with:");

        sb.AppendLine();

        sb.AppendLine(
            $"\"{PromptConstants.NotFoundResponse}\"");

        sb.AppendLine();

        if (history.Any())
        {
            sb.AppendLine(
                "Conversation History:");

            sb.AppendLine();

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
            "Retrieved Document Context:");

        sb.AppendLine();

        sb.AppendLine(context);

        sb.AppendLine();

        sb.AppendLine(
            $"Question: {question}");

        return sb.ToString();
    }
}