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
}