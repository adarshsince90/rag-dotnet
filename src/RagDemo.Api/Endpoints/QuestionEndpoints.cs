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

        return endpoints;
    }
}