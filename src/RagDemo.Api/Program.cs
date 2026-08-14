var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        Project = "RagDemo",
        Sprint = "0",
        Status = "Ready"
    });
});

app.MapGet("/architecture", () =>
{
    return Results.Ok(new
    {
        Layers = new[]
        {
            "Api",
            "Application",
            "Domain",
            "Infrastructure"
        }
    });
});

app.Run();

