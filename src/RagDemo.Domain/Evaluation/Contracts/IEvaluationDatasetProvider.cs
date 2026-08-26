using RagDemo.Domain.Evaluation.Models;

namespace RagDemo.Domain.Evaluation.Contracts;

public interface IEvaluationDatasetProvider
{
    Task<IReadOnlyCollection<EvaluationTestCase>>
        LoadAsync(
            CancellationToken cancellationToken = default);
}