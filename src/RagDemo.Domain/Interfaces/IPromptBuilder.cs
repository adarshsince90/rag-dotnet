namespace RagDemo.Domain.Interfaces;

public interface IPromptBuilder
{
    string BuildPrompt(
        string question,
        string context);
}