namespace RagDemo.Infrastructure.Configuration;

public sealed class ProviderOptions
{
    public string Type
    {
        get;
        init;
    } = string.Empty;

    public string BaseUrl
    {
        get;
        init;
    } = string.Empty;

    public string ApiKey
    {
        get;
        init;
    } = string.Empty;

    public string ChatModel
    {
        get;
        init;
    } = string.Empty;

    public string? EmbeddingModel
    {
        get;
        init;
    }

    public int MaxRetryAttempts
    {
        get;
        init;
    } = 3;

    public int RetryDelaySeconds
    {
        get;
        init;
    } = 15;
}