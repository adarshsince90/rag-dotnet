using RagDemo.Application.Dtos;
using RagDemo.Application.Services;

public static class DiagnosticEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/diagnostic/health",
            () => Results.Ok(new
            {
                Status = "Healthy"
            }));

        return endpoints;
    }
}