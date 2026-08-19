public record QuestionAnswerResponse(
    string Question,
    string Answer,
    RetrievalDiagnosticsResponse Diagnostics,
    IReadOnlyCollection<MatchResponse> Matches);