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

        return endpoints;
    }
}