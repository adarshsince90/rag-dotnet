using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using RagDemo.Application.Constants;
using RagDemo.Application.Services;
using RagDemo.Domain.Contracts;

public sealed class ConversationQuestionAnsweringService
{
    private readonly RetrievalService _retrievalService;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IPromptBuilder _promptBuilder;
    private readonly IConversationMemory _conversationMemory;
    private readonly IConversationQueryBuilder _queryBuilder;
    private readonly IQueryClassifier _queryClassifier;
    private readonly ILogger<ConversationQuestionAnsweringService> _logger;

    public ConversationQuestionAnsweringService(
        RetrievalService retrievalService,
        IChatCompletionService chatCompletionService,
        IPromptBuilder promptBuilder,
        IConversationMemory conversationMemory,
        IConversationQueryBuilder conversationQueryBuilder,
        IQueryClassifier queryClassifier,
        ILogger<ConversationQuestionAnsweringService> logger)
    {
        _retrievalService = retrievalService;
        _chatCompletionService = chatCompletionService;
        _promptBuilder = promptBuilder;
        _conversationMemory = conversationMemory;
        _queryBuilder = conversationQueryBuilder;
        _queryClassifier = queryClassifier;
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

        if (!requestContext.RequiresRetrieval)
        {
            yield return new StreamingEvent
            {
                EventType = "token",
                Payload =
                    requestContext.DirectResponse!
            };

            yield return new StreamingEvent
            {
                EventType = "completed",
                Payload = new GenerationDiagnostics
                {
                    GenerationMs = 0,
                    TotalMs = 0,
                    AnswerCharacters =
                        requestContext.DirectResponse?.Length ?? 0
                }
            };

            yield break;
        }


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

        if (requestContext.ShouldPersistConversation)
        {
            await _conversationMemory.AddTurnAsync(
                conversationId,
                new ConversationTurn
                {
                    Question = question,
                    Answer = answerBuilder.ToString()
                },
                cancellationToken);
        }
    }

    private async Task<ConversationRequestContext>
    BuildRequestContextAsync(
        string conversationId,
        string question,
        CancellationToken cancellationToken)
    {
        var classification =
            _queryClassifier.Classify(question);

        if (classification.QueryType != QueryType.DocumentQuestion)
        {
            return new ConversationRequestContext
            {
                RequiresRetrieval = false,
                ShouldPersistConversation = false,

                DirectResponse =
                    classification.Response,

                Prompt = string.Empty,

                Context = string.Empty,

                Question = question,

                Matches = [],

                History = [],

                ConversationDiagnostics =
                    new ConversationDiagnostics(
                        conversationId,
                        0,
                        0,
                        0),

                RetrievalMetrics =
                    new RetrievalMetrics(),

                PromptDiagnostics =
                    new PromptDiagnostics(
                        0,
                        0,
                        0)
            };
        }

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

        if (!retrievalResponse.Matches.Any()
                && !history.Any())
        {
            return new ConversationRequestContext
            {
                RequiresRetrieval = false,

                ShouldPersistConversation = false,

                DirectResponse =
                    PromptConstants.NotFoundResponse,

                Prompt = string.Empty,

                Context = string.Empty,

                Question = question,

                Matches = [],

                History = history,

                ConversationDiagnostics =
                    conversationDiagnostics,

                RetrievalMetrics =
                    new RetrievalMetrics(),
                    
                PromptDiagnostics =
                    new PromptDiagnostics(
                        0,
                        0,
                        0)
            };
        }

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

        var shouldPersistConversation =
                retrievalResponse.Matches.Any() || history.Any();

        return new ConversationRequestContext
        {
            RequiresRetrieval = true,
            ShouldPersistConversation = shouldPersistConversation,
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

        if (!requestContext.RequiresRetrieval)
        {
            return new QuestionAnswerResponse(
                Question: question,
                Answer:
                    requestContext.DirectResponse!,
                Diagnostics:
                    new RetrievalDiagnosticsResponse(
                        QualifiedChunks: 0,
                        ReturnedChunks: 0,
                        TopK: 0,
                        RetrievalMs: 0,
                        GenerationMs: 0),
                Matches: []);
        }

        var generationStopwatch = Stopwatch.StartNew();

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
