namespace RagDemo.Infrastructure.Configuration;

public sealed class DataOptions
{
    public string InputFile { get; init; } = string.Empty;

    public string InputFolder { get; init; } = string.Empty;

    public string OutputFolder { get; init; } = string.Empty;

    public string PdfDirectory { get; init; } = string.Empty;
}