namespace RagDemo.Domain.Models;
public sealed class RetrievalResponse
{
    public required IReadOnlyCollection<RetrievalResult> Results { get; init; }
    public required RetrievalDiagnostics Diagnostics { get; set; }
}