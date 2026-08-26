public static class EvaluationEndpoints
{
    public static IEndpointRouteBuilder MapEvaluationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/evaluation/run",
            async (
                IEvaluationService service,
                CancellationToken cancellationToken) =>
            {
                var result =
                    await service.RunAsync(
                        cancellationToken);

                return Results.Ok(result);
            });

        return endpoints;
    }
}