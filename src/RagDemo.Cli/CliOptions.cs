public sealed class CliOptions
{
    public string ApiUrl { get; set; }
        = "http://localhost:5000/conversation/stream";

    public string ConversationId { get; set; }
        = Guid.NewGuid().ToString();
}