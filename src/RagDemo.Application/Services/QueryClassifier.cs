public sealed class QueryClassifier
    : IQueryClassifier
{
    private static readonly HashSet<string>
        Greetings =
        [
            "hi",
            "hello",
            "hey",
            "good morning",
            "good afternoon",
            "good evening",
            "how are you",
            "thanks",
            "thank you",
            "bye"
        ];

    private static readonly HashSet<string>
    CapabilityQuestions =
    [
        "what can you do",
        "who are you",
        "what is your name",
        "help"
    ];

    public QueryClassificationResult Classify(
        string question)
    {
        var normalized =
            question
                .Trim()
                .ToLowerInvariant();

        if (Greetings.Contains(normalized))
        {
            return new QueryClassificationResult(
                QueryType.Greeting,
                ChatResponses.Greeting);
        }

        if (CapabilityQuestions.Contains(normalized))
        {
            return new QueryClassificationResult(
                QueryType.Capabilities,
                ChatResponses.Capabilities);
        }

        return new QueryClassificationResult(
            QueryType.DocumentQuestion);
    }
}