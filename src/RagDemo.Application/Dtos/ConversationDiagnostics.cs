public sealed record ConversationDiagnostics(
    string ConversationId,
    int TurnsUsed,
    int HistoryCharacters,
    int RetrievalQueryCharacters);