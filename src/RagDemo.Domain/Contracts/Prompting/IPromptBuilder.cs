namespace RagDemo.Domain.Contracts;

public interface IPromptBuilder
{
    string BuildPrompt(
        string question,
        string context);
}