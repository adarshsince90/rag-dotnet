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

    public async IAsyncEnumerable<string> AskConversationStreamAsync(
        string conversationId,
        string question, 
        [EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AskStreamAsync started");

        var history =
            await _conversationMemory
            .GetRecentTurnsAsync(
            conversationId,
            cancellationToken);

        var retrievalQuery =
            _queryBuilder.BuildQuery(
            question,
            history);
        
        var retrievalResponse =
            await _retrievalService
                .RetrieveAsync(
                    retrievalQuery,
                    cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} chunks",
            retrievalResponse.Matches.Count);


        var context = 
            BuildContext(
                retrievalResponse.Matches);

        var prompt =
            _promptBuilder
                .BuildConversationPrompt(
                    question,
                    context,
                    history);

        var answerBuilder =
            new StringBuilder();

        await foreach 
            (var token in 
                _chatCompletionService .GenerateStreamingAsync(
                    prompt, 
                    cancellationToken))
                    {
                        answerBuilder.Append(token);
                        yield return token;
                    }
        
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

    private static string BuildContext(
        IReadOnlyCollection<MatchResponse> matches)
    {
        return string.Join(
            Environment.NewLine,
            matches.Select(x => x.Content));
    }
}
