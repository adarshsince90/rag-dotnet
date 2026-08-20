using System.Diagnostics;
using RagDemo.Application.Services;
using RagDemo.Domain.Contracts;

public sealed class QuestionAnsweringService
{
    private readonly RetrievalService _retrievalService;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IPromptBuilder _promptBuilder;

    public QuestionAnsweringService(
        RetrievalService retrievalService,
        IChatCompletionService chatCompletionService,
        IPromptBuilder promptBuilder)
    {
        _retrievalService = retrievalService;
        _chatCompletionService = chatCompletionService;
        _promptBuilder = promptBuilder;
    }

    public async Task<QuestionAnswerResponse> AskAsync(string question, CancellationToken cancellationToken = default)
    {
        var retrievalStopwatch = Stopwatch.StartNew();
        
        var retrievalResponse =
            await _retrievalService
                .RetrieveAsync(
                    question,
                    cancellationToken);

        retrievalStopwatch.Stop();

        if (retrievalResponse.Matches == null || !retrievalResponse.Matches.Any())
        {
            return new QuestionAnswerResponse(
                question,
                "I could not find the answer in the provided documents.",
                retrievalResponse.Diagnostics with
                {
                    RetrievalMs = retrievalStopwatch.ElapsedMilliseconds,
                    GenerationMs = 0,
                    TotalMs = (int)retrievalStopwatch.ElapsedMilliseconds,
                    llmInvoked = false
                },
                Array.Empty<MatchResponse>());
        }

        var generationStopwatch = Stopwatch.StartNew();
        var context = BuildContext(retrievalResponse.Matches);

        var prompt =
            _promptBuilder.BuildPrompt(
            question,
            context);

        var answer =
            await _chatCompletionService
                .GenerateAnswerAsync(
                    prompt,
                    cancellationToken);

        generationStopwatch.Stop();
        
        var answerText = answer?.Trim() ?? "I could not generate an answer.";

        return new QuestionAnswerResponse(
            question,
            answerText,
            retrievalResponse.Diagnostics with
            {
                RetrievalMs = retrievalStopwatch.ElapsedMilliseconds,
                GenerationMs = generationStopwatch.ElapsedMilliseconds,
                TotalMs = (int)(retrievalStopwatch.ElapsedMilliseconds + generationStopwatch.ElapsedMilliseconds),
                llmInvoked = true
            },
            retrievalResponse.Matches);
    }

    private static string BuildContext(
        IReadOnlyCollection<MatchResponse> matches)
    {
        return string.Join(
            Environment.NewLine,
            matches.Select(x => x.Content));
    }
}
