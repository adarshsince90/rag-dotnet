using RagDemo.Application.Dtos;
using RagDemo.Application.Services;

public static class RetrievalEndpoints
{
    public static IEndpointRouteBuilder MapRetrievalEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/retrieval/search",
            async (
            AskQuestionRequest request,
            AskQuestionService service,
            CancellationToken cancellationToken) =>
                {
                    var response = await service.AskAsync(
                        request.Question,
                        cancellationToken);

                    return Results.Ok(response);
                });
        return endpoints;
    }
}