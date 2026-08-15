namespace RagDemo.Application.Dtos;

public record AskQuestionResponse(string Question, string? Match, double Score);