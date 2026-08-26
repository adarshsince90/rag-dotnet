using System.Text.Json;

public static class ConversationEndpoints
{
    public static IEndpointRouteBuilder MapConversationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api-conversation/stream",
            async (
                HttpContext context,
                AskConversationRequest request,
                ConversationQuestionAnsweringService service,
                CancellationToken cancellationToken) =>
        {
          context.Response.ContentType =
                "text/event-stream";
            
            context.Response.Headers.Append(
                "Cache-Control",
                "no-cache");
                
            context.Response.Headers.Append(
                "Connection",
                "keep-alive");
                
            await foreach (var streamEvent in service
                .AskConversationStreamAsync(
                    request.ConversationId, 
                    request.Question, 
                    cancellationToken).WithCancellation(cancellationToken))
                {
                    // await context.Response.WriteAsync(
                    //     $"{token}",
                    //     cancellationToken);

                    // await context.Response.Body.FlushAsync(
                    //     cancellationToken);

                    await WriteEventAsync(
                        context.Response,
                        streamEvent.EventType,
                        streamEvent.Payload,
                        cancellationToken);
                }
        });

        return endpoints;
    }

    private static async Task WriteEventAsync<T>(
        HttpResponse response,
        string eventType,
        T payload,
        CancellationToken cancellationToken)
    {
        if (eventType == "token")
        {
            await response.WriteAsync(
                        $"{payload}",
                        cancellationToken);
        }
        else 
        {
            var json =
                JsonSerializer.Serialize(payload);

            await response.WriteAsync(
                $"\nevent: {eventType}\n",
                cancellationToken);

            await response.WriteAsync(
                $"data: {json}\n\n",
                cancellationToken);

            await response.Body.FlushAsync(
                cancellationToken);
        }
    }
}