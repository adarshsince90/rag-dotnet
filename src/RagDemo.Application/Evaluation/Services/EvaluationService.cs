using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using RagDemo.Application.Constants;
using RagDemo.Domain.Evaluation.Contracts;
using RagDemo.Domain.Evaluation.Models;

namespace RagDemo.Application.Evaluation.Services;

public sealed class EvaluationService
    : IEvaluationService
{
    private readonly IEvaluationDatasetProvider _datasetProvider;
    private readonly QuestionAnsweringService _questionAnsweringService;
    private readonly ConversationQuestionAnsweringService _conversationService;
    private readonly ILogger<EvaluationService> _logger;

    public EvaluationService(
        IEvaluationDatasetProvider datasetProvider,
        QuestionAnsweringService questionAnsweringService,
        ConversationQuestionAnsweringService conversationService,
        ILogger<EvaluationService> logger)
    {
        _datasetProvider = datasetProvider;
        _questionAnsweringService = questionAnsweringService;
        _conversationService = conversationService;
        _logger = logger;
    }

    public async Task<EvaluationSummary> RunAsync(
        CancellationToken cancellationToken = default)
    {
        var dataset =
            await _datasetProvider.LoadAsync(
                cancellationToken);

        var results =
            new List<EvaluationResult>();

        foreach (var testCase in dataset)
        {
            _logger.LogInformation(
                "Evaluating Question: {Question}",
                testCase.Question);

            QuestionAnswerResponse? response = null;
            foreach (var historyQuestion in testCase.History)
            {
                await _conversationService
                    .AskAsync(
                        testCase.ConversationId,
                        historyQuestion,
                        cancellationToken);
            }

            response =
                await _conversationService
                    .AskAsync(
                        testCase.ConversationId,
                        testCase.Question,
                        cancellationToken);
            
            if (response is null)
            {
                continue;
            }

            var topMatch =
                response.Matches.FirstOrDefault();

            var actualSource = topMatch?.Source;

            var sourceMatched =
                testCase.Category == "grounding"
                    ? true
                    : string.Equals(
                        actualSource,
                        testCase.ExpectedSource,
                        StringComparison.OrdinalIgnoreCase);

            var answerLower =
                response.Answer
                    .ToLowerInvariant();

            var normalizedAnswer =
                    Regex.Replace(
                        response.Answer,
                        @"\s+",
                        " ")
                    .Trim();
                    
            var groundingPassed =
                !testCase.ExpectGroundedRefusal
                ||
                normalizedAnswer.Contains(
                    PromptConstants.NotFoundMarker,
                    StringComparison.OrdinalIgnoreCase);

            var matchedKeywords =
                testCase.ExpectedKeywords
                    .Count(keyword =>
                        answerLower.Contains(
                            keyword.ToLowerInvariant()));

            var keywordMatched =
                    matchedKeywords == testCase.ExpectedKeywords.Count;

            var totalKeywords = testCase.ExpectedKeywords.Count;

            var keywordCoverage = 
                    totalKeywords == 0 
                    ? 1.0 
                    : (double)matchedKeywords /totalKeywords;

            var retrievedSources =
                    response.Matches
                        .Select(x => x.Source)
                        .Distinct()
                        .ToList();

            var expectedSourceFound =
                string.IsNullOrWhiteSpace(
                    testCase.ExpectedSource)
                ||
                retrievedSources.Any(source =>
                    source.Equals(
                        testCase.ExpectedSource,
                        StringComparison.OrdinalIgnoreCase));
            
           var result =
                new EvaluationResult
                {
                    TestCaseId = testCase.Id,
                    Category = testCase.Category,
                    ConversationId = testCase.ConversationId,
                    FinalQuestion = testCase.Question,
                    ExpectedSource = testCase.ExpectedSource,
                    RetrievedSources = retrievedSources,
                    ExpectedSourceFound = expectedSourceFound,
                    MatchedKeywords = matchedKeywords,
                    TotalKeywords = totalKeywords,
                    KeywordCoverage = keywordCoverage,
                    HighestScore = topMatch?.Score ?? 0,
                    GeneratedAnswer = response.Answer,
                    RetrievalMs = response.Diagnostics.RetrievalMs,
                    GenerationMs = response.Diagnostics.GenerationMs,
                    GroundingPassed = groundingPassed
                };
                results.Add(result);

           _logger.LogInformation("""
                    Evaluation completed.
                    TestCaseId={TestCaseId},
                    ExpectedSourceFound={ExpectedSourceFound},
                    KeywordCoverage={KeywordCoverage:P2}
                    """,
                    result.TestCaseId,
                    result.ExpectedSourceFound,
                    result.KeywordCoverage);
        }

        return BuildSummary(results);
    }

    private static EvaluationSummary BuildSummary(
    List<EvaluationResult> results)
    {
        var expectedSourceFoundCount =
            results.Count(x =>
                x.ExpectedSourceFound);

        var groundingPasses =
            results.Count(x =>
                x.GroundingPassed);

        var evaluationSummary = new EvaluationSummary
        {
            TotalQuestions =
                results.Count,

            ExpectedSourceFoundCount =
                expectedSourceFoundCount,

            ExpectedSourceCoverage =
                results.Count == 0
                    ? 0
                    : (double)
                        expectedSourceFoundCount /
                        results.Count,

            AverageKeywordCoverage =
                results.Any()
                    ? results.Average(
                        x => x.KeywordCoverage)
                    : 0,

            GroundingPasses =
                groundingPasses,

            GroundingAccuracy =
                results.Count == 0
                    ? 0
                    : (double)
                        groundingPasses /
                        results.Count,

            AverageRetrievalMs =
                results.Any()
                    ? results.Average(
                        x => x.RetrievalMs)
                    : 0,

            AverageGenerationMs =
                results.Any()
                    ? results.Average(
                        x => x.GenerationMs)
                    : 0,

            AverageHighestScore =
                results.Any()
                    ? results.Average(
                        x => x.HighestScore)
                    : 0,

            Results = results
        };

        return evaluationSummary;
    }
}