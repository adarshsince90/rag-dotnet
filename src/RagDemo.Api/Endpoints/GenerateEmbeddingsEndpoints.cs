using Microsoft.AspNetCore.Mvc;
using RagDemo.Application.Services;

public static class GenerateEmbeddingsEndpoints
{
    public static IEndpointRouteBuilder MapGenerateEmbeddingsEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/embeddings/generate",
            async (
                [FromServices]
                GenerateEmbeddingsService service,
                CancellationToken cancellationToken) =>
        {
            var chunks =
                await service.GenerateEmbeddingsAsync(
                    cancellationToken);

            return Results.Ok(new
            {
                Chunks = chunks.Count
            });
        });

        return endpoints;
    }
}