using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using RagDemo.Application.Services;
using RagDemo.Domain.Contracts;

public sealed class ConversationQuestionAnsweringService
{
    private readonly RetrievalService _retrievalService;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IPromptBuilder _promptBuilder;
    private readonly IConversationMemory _conversationMemory;
    private readonly IConversationQueryBuilder _queryBuilder;
    private readonly ILogger<ConversationQuestionAnsweringService> _logger;

    public ConversationQuestionAnsweringService(
        RetrievalService retrievalService,
        IChatCompletionService chatCompletionService,
        IPromptBuilder promptBuilder,
        IConversationMemory conversationMemory,
        IConversationQueryBuilder conversationQueryBuilder,
        ILogger<ConversationQuestionAnsweringService> logger)
    {
        _retrievalService = retrievalService;
        _chatCompletionService = chatCompletionService;
        _promptBuilder = promptBuilder;
        _conversationMemory = conversationMemory;
        _queryBuilder = conversationQueryBuilder;
        _logger = logger;
    }

    public async IAsyncEnumerable<StreamingEvent> AskConversationStreamAsync(
        string conversationId,
        string question, 
        [EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AskStreamAsync started");
        
        var totalStopwatch = Stopwatch.StartNew();

        var requestContext =
            await BuildRequestContextAsync(
                conversationId,
                question,
                cancellationToken);


        yield return new StreamingEvent
        {
            EventType = "conversation",
            Payload =
                requestContext
                    .ConversationDiagnostics
        };

        yield return new StreamingEvent
        {
            EventType = "retrieval",
            Payload =
                requestContext
                    .RetrievalMetrics
        };

        _logger.LogInformation(
            "Retrieved {Count} chunks",
            requestContext.RetrievalMetrics.ReturnedChunks);

       yield return new StreamingEvent
        {
            EventType = "prompt",
            Payload =
                requestContext
                    .PromptDiagnostics
        };

        var answerBuilder =
            new StringBuilder();

        var generationStopwatch = Stopwatch.StartNew();

        await foreach 
            (var token in 
                _chatCompletionService .GenerateStreamingAsync(
                    requestContext.Prompt, 
                    cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        answerBuilder.Append(token);
                        // yield return token;
                        yield return new StreamingEvent
                        {
                            EventType = "token",
                            Payload = token
                        };
                    }
        
        generationStopwatch.Stop();
        totalStopwatch.Stop();

        yield return new StreamingEvent
        {
            EventType = "completed",
            Payload = new GenerationDiagnostics
            {
                GenerationMs = generationStopwatch.ElapsedMilliseconds,
                TotalMs = totalStopwatch.ElapsedMilliseconds,
                AnswerCharacters = answerBuilder.Length
            }
        };

        await _conversationMemory.AddTurnAsync(
            conversationId,
            new ConversationTurn
            {
                Question = question,
                Answer = answerBuilder.ToString()
            },
            cancellationToken
        );
    }

    private async Task<ConversationRequestContext>
    BuildRequestContextAsync(
        string conversationId,
        string question,
        CancellationToken cancellationToken)
    {
        var history =
            await _conversationMemory
                .GetRecentTurnsAsync(
                    conversationId,
                    cancellationToken);

        var retrievalQuery =
            _queryBuilder.BuildQuery(
                question,
                history);

        var conversationDiagnostics =
            new ConversationDiagnostics(
                ConversationId: conversationId,
                TurnsUsed: history.Count,
                HistoryCharacters:
                    history.Sum(x =>
                        x.Question.Length +
                        x.Answer.Length),
                RetrievalQueryCharacters:
                    retrievalQuery.Length);

        var retrievalStopwatch = Stopwatch.StartNew();
        var retrievalResponse =
            await _retrievalService
                .RetrieveAsync(
                    question,//retrievalQuery,
                    cancellationToken);
        retrievalStopwatch.Stop();

        var scores =
            retrievalResponse.Matches
                .Select(x => x.Score)
                .ToArray();

        var retrievalMetrics =
            new RetrievalMetrics
            {
                RetrievalMs = retrievalStopwatch.ElapsedMilliseconds,
                ReturnedChunks =
                    retrievalResponse.Matches.Count,

                AverageScore =
                    scores.Length > 0
                        ? scores.Average()
                        : 0,

                HighestScore =
                    scores.Length > 0
                        ? scores.Max()
                        : 0,

                LowestScore =
                    scores.Length > 0
                        ? scores.Min()
                        : 0,
                Sources =
                    retrievalResponse.Matches
                        .Select(x => x.Source)
                        .Distinct()
                        .ToList()
            };

        var context =
            BuildContext(
                retrievalResponse.Matches);

        var prompt =
            _promptBuilder
                .BuildConversationPrompt(
                    question,
                    context,
                    history);

        var promptDiagnostics =
            new PromptDiagnostics(
                ContextCharacters: context.Length,
                PromptCharacters: prompt.Length,
                RetrievedChunkCount:
                    retrievalResponse.Matches.Count);

        return new ConversationRequestContext
        {
            Prompt = prompt,
            Context = context,
            Question = question,
            Matches =
                retrievalResponse.Matches,

            History = history,

            ConversationDiagnostics =
                conversationDiagnostics,

            RetrievalMetrics =
                retrievalMetrics,

            PromptDiagnostics =
                promptDiagnostics
        };
    }

    public async Task<QuestionAnswerResponse>
    AskAsync(
        string conversationId,
        string question,
        CancellationToken cancellationToken = default)
    {
        var requestContext =
            await BuildRequestContextAsync(
                conversationId,
                question,
                cancellationToken);

        var generationStopwatch =
            Stopwatch.StartNew();

        var answer =
            await _chatCompletionService
                .GenerateAnswerAsync(
                    requestContext.Prompt,
                    cancellationToken);

        generationStopwatch.Stop();

        await _conversationMemory
            .AddTurnAsync(
                conversationId,
                new ConversationTurn
                {
                    Question = question,
                    Answer = answer
                },
                cancellationToken);

        return new QuestionAnswerResponse(
            Question: question,
            Answer: answer,
            Diagnostics:
                new RetrievalDiagnosticsResponse(
                    //TotalChunks: 0,
                    QualifiedChunks:
                        requestContext.Matches.Count,
                    ReturnedChunks:
                        requestContext.Matches.Count,
                    TopK:
                        requestContext
                            .Matches.Count,
                    RetrievalMs: 
                        requestContext
                            .RetrievalMetrics.RetrievalMs,
                    GenerationMs:
                        generationStopwatch
                            .ElapsedMilliseconds),
            Matches:
                requestContext
                    .Matches
                    .ToList());
    }

    private static string BuildContext(
    IReadOnlyCollection<MatchResponse> matches)
    {
        var sb = new StringBuilder();

        foreach(var match in matches)
        {
            sb.AppendLine(
                $"Source: {match.Source}");

            sb.AppendLine(
                match.Content);

            sb.AppendLine();
        }

        return sb.ToString();
    }

    //  private static string BuildContext(
    //     IReadOnlyCollection<MatchResponse> matches)
    // {
    //     return string.Join(
    //         Environment.NewLine,
    //         matches.Select(x => x.Content));
    // }

    // public async IAsyncEnumerable<StreamingEvent> AskConversationStreamAsync(
    //     string conversationId,
    //     string question, 
    //     [EnumeratorCancellation]
    //     CancellationToken cancellationToken = default)
    // {
    //     _logger.LogInformation(
    //         "AskStreamAsync started");

    //     var history =
    //         await _conversationMemory
    //         .GetRecentTurnsAsync(
    //         conversationId,
    //         cancellationToken);

    //     var retrievalQuery =
    //         _queryBuilder.BuildQuery(
    //         question,
    //         history);

    //     yield return new StreamingEvent
    //     {
    //         EventType = "conversation",
    //         Payload = new ConversationDiagnostics
    //         (
    //             ConversationId: conversationId,
    //             TurnsUsed: history.Count,
    //             HistoryCharacters: history.Sum(x=>
    //                 x.Question.Length + 
    //                 x.Answer.Length),
    //             RetrievalQueryCharacters: retrievalQuery.Length
    //         )
    //     };
        
    //     var retrievalResponse =
    //         await _retrievalService
    //             .RetrieveAsync(
    //                 retrievalQuery,
    //                 cancellationToken);

    //     var scores =
    //         retrievalResponse.Matches
    //         .Select(x => x.Score)
    //         .ToList();

    //     yield return new StreamingEvent
    //         {
    //             EventType = "retrieval",
    //             Payload = new RetrievalMetrics
    //             {
    //                 ReturnedChunks = retrievalResponse.Matches.Count,
    //                 AverageScore = scores.Any() ? scores.Average() : 0,
    //                 HighestScore =
    //                 scores.Any() ? scores.Max() : 0,
    //                 LowestScore = scores.Any() ? scores.Min() : 0
    //             }
    //         };

    //     _logger.LogInformation(
    //         "Retrieved {Count} chunks",
    //         retrievalResponse.Matches.Count);


    //     var context = 
    //         BuildContext(
    //             retrievalResponse.Matches);

    //     var prompt =
    //         _promptBuilder
    //             .BuildConversationPrompt(
    //                 question,
    //                 context,
    //                 history);

    //     yield return new StreamingEvent
    //     {
    //         EventType = "prompt",
    //         Payload = new PromptDiagnostics
    //         (
    //             ContextCharacters: context.Length,
    //             PromptCharacters: prompt.Length,
    //             RetrievedChunkCount: retrievalResponse.Matches.Count
    //         )
    //     };

    //     var answerBuilder =
    //         new StringBuilder();

    //     var generationStopwatch = Stopwatch.StartNew();

    //     await foreach 
    //         (var token in 
    //             _chatCompletionService .GenerateStreamingAsync(
    //                 prompt, 
    //                 cancellationToken))
    //                 {
    //                     answerBuilder.Append(token);
    //                     // yield return token;
    //                     yield return new StreamingEvent
    //                     {
    //                         EventType = "token",
    //                         Payload = token
    //                     };
    //                 }
        
    //     generationStopwatch.Stop();

    //     yield return new StreamingEvent
    //     {
    //         EventType = "completed",
    //         Payload = new GenerationDiagnostics
    //         {
    //             GenerationMs =
    //                 generationStopwatch.ElapsedMilliseconds,
    //             AnswerCharacters =
    //                 answerBuilder.Length
    //         }
    //     };

    //     await _conversationMemory.AddTurnAsync(
    //         conversationId,
    //         new ConversationTurn
    //         {
    //             Question = question,
    //             Answer = answerBuilder.ToString()
    //         },
    //         cancellationToken
    //     );
    // }
}
