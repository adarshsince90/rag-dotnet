using System.Text.Json;
using Microsoft.Extensions.Options;
using RagDemo.Domain.Evaluation.Contracts;
using RagDemo.Domain.Evaluation.Models;

public sealed class JsonEvaluationDatasetProvider
    : IEvaluationDatasetProvider
{
    private readonly EvaluationOptions _options;

    public JsonEvaluationDatasetProvider(
        IOptions<EvaluationOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IReadOnlyCollection<EvaluationTestCase>>
        LoadAsync(
            CancellationToken cancellationToken = default)
    {
        var json =
            await File.ReadAllTextAsync(
                _options.DatasetPath,
                cancellationToken);

        return JsonSerializer.Deserialize<
            List<EvaluationTestCase>>(json)
            ?? [];
    }
}