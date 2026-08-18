using Microsoft.AspNetCore.Mvc;
using RagDemo.Application.Dtos;
using RagDemo.Application.Services;
using RagDemo.Domain.Abstractions;
using RagDemo.Domain.Interfaces;
using RagDemo.Infrastructure.Documents;
using RagDemo.Infrastructure.Embedding;
using RagDemo.Infrastructure.Retrieval;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IChunkProvider,
    TextFileChunkProvider>();

builder.Services.AddScoped<IRetriever,
    VectorRetriever>();

builder.Services.AddHttpClient<IEmbeddingGenerator,
    OllamaEmbeddingGenerator>();

builder.Services.AddScoped<AskQuestionService>();

builder.Services.AddScoped<GenerateEmbeddingsService>();

builder.Services.Configure<OllamaOptions>(
    builder.Configuration.GetSection("Ollama"));

builder.Services.Configure<DataOptions>(
    builder.Configuration.GetSection("Data"));

builder.Services.Configure<RetrievalOptions>(
    builder.Configuration.GetSection("Retrieval"));

builder.Services.AddSingleton<IChunkStore,
    InMemoryChunkStore>();

builder.Services.AddScoped<IRetrievalResultProcessor,
    RetrievalResultProcessor>();

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

app.MapPost("/embeddings/generate",
    async (
        [FromServices] GenerateEmbeddingsService service,
        CancellationToken cancellationToken) =>
{
    var chunks = await service
        .GenerateEmbeddingsAsync(cancellationToken);

    return Results.Ok(new
    {
        Chunks = chunks.Count
    });
});

app.Run();

