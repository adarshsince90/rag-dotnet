using RagDemo.Application.Dtos;
using RagDemo.Application.Services;
using RagDemo.Domain.Abstractions;
using RagDemo.Infrastructure.Retrieval;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<IChunkProvider,
    TextFileChunkProvider>();

builder.Services.AddScoped<IRetriever,
    KeywordRetriever>();

builder.Services.AddScoped<AskQuestionService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
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

app.MapPost("/ask",
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

app.Run();

