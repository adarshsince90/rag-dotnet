using RagDemo.Domain.Evaluation.Models;

public interface IEvaluationService
{
    Task<EvaluationSummary> RunAsync(
        CancellationToken cancellationToken = default);
}