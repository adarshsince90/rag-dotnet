namespace RagDemo.Infrastructure.Configuration;

public sealed class AiOptions
{
    public string DefaultProvider
    {
        get;
        init;
    } = string.Empty;

    public Dictionary<string, ProviderOptions>
        Providers
    {
        get;
        init;
    } = [];
}