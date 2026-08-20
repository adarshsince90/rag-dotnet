using RagDemo.Infrastructure.Documents.Extraction;

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

        endpoints.MapGet("/test",
            async ()=>
            {
                var extractor = new PdfDocumentExtractor();

                var text =
                    await extractor.ExtractTextAsync("../../data/raw/pdf/attention-is-what-you-need.pdf");

                return Results.Ok(new
                {
                   Response = text[..1000]
                });        
            });

        return endpoints;
    }
}