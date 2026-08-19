using System.Diagnostics;
using RagDemo.Application.Services;
using RagDemo.Domain.Interfaces;

public sealed class QuestionAnsweringService
{
    private readonly AskQuestionService _retrievalService;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IPromptBuilder _promptBuilder;

    public QuestionAnsweringService(
        AskQuestionService retrievalService,
        IChatCompletionService chatCompletionService,
        IPromptBuilder promptBuilder)
    {
        _retrievalService = retrievalService;
        _chatCompletionService = chatCompletionService;
        _promptBuilder = promptBuilder;
    }

    public async Task<QuestionAnswerResponse> AskAsync(
    string question,
    CancellationToken cancellationToken = default)
    {
        var retrievalStopwatch = Stopwatch.StartNew();
        
        var retrievalResponse =
            await _retrievalService
                .AskAsync(
                    question,
                    cancellationToken);

        retrievalStopwatch.Stop();

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
        
        var answerText = answer?.Trim() ?? string.Empty;
        if (!retrievalResponse.Matches.Any())
        {
            answerText = "I could not find the answer in the provided documents.";
        }

       return new QuestionAnswerResponse(
            question,
            answerText,
            retrievalResponse.Diagnostics with
            {
                RetrievalMs = retrievalStopwatch.ElapsedMilliseconds,
                GenerationMs = generationStopwatch.ElapsedMilliseconds,
                TotalMs = (int)(retrievalStopwatch.ElapsedMilliseconds + generationStopwatch.ElapsedMilliseconds)
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
