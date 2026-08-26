using System.Text.Json;
using RagDemo.Application.Dtos;

public static class QuestionEndpoints
{
    public static IEndpointRouteBuilder MapQuestionEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/ask",
            async (
                AskQuestionRequest request,
                QuestionAnsweringService service,
                CancellationToken cancellationToken) =>
        {
            var response =
                await service.AskAsync(
                    request.Question,
                    cancellationToken);

            return Results.Ok(response);
        });

        endpoints.MapPost("/ask/stream",
            async (
                HttpContext context,
                AskQuestionRequest request,
                QuestionAnsweringService service,
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
                
            await foreach (var token in service
                .AskStreamAsync(request.Question, cancellationToken).WithCancellation(cancellationToken))
                {
                    await context.Response.WriteAsync(
                        $"data: {token}\n\n",
                        cancellationToken);

                    await context.Response.Body.FlushAsync(
                        cancellationToken);
                }
        });

        endpoints.MapPost("/conversation/stream",
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
        var json =
            JsonSerializer.Serialize(payload);

        await response.WriteAsync(
            $"event: {eventType}\n",
            cancellationToken);

        await response.WriteAsync(
            $"data: {json}\n\n",
            cancellationToken);

        await response.Body.FlushAsync(
            cancellationToken);
    }
}