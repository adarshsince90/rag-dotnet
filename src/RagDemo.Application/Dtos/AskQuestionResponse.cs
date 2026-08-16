namespace RagDemo.Application.Dtos;

// public record AskQuestionResponse(string Question, string? Match, double Score);

// public record AskQuestionResponse(
// string Question,
// IReadOnlyCollection<MatchResponse> Matches);

// public record AskQuestionResponse(
//     string Question,
//     IReadOnlyCollection<MatchResponse> Matches);

public record AskQuestionResponse(
    string Question,
    RetrievalDiagnosticsResponse Diagnostics,
    IReadOnlyCollection<MatchResponse> Matches);