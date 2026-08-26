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
            "Answer ONLY using the provided context.");

        sb.AppendLine();

        sb.AppendLine(
            "You may combine and summarize information from multiple parts of the provided context when answering.");
         
         sb.AppendLine(
            "Do not use prior knowledge.");
        
        sb.AppendLine(
            $"If the answer cannot reasonably be determined from the provided context, reply EXACTLY with:");

        sb.AppendLine(
            $"\"{PromptConstants.NotFoundResponse}\"");

        sb.AppendLine();

        // sb.AppendLine(
        //     "Do not make assumptions.");

        // sb.AppendLine(
        //     "Do not infer information that is not present in the context.");

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