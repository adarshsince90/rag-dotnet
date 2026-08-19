using Scalar.AspNetCore;
using RagDemo.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services
	.AddOptionsConfiguration(builder.Configuration)
	.AddApplicationServices()
	.AddInfrastructureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapDiagnosticEndpoints();
app.MapRetrievalEndpoints();
app.MapGenerateEmbeddingsEndpoints();
app.MapQuestionEndpoints();

await StartupTasks.InitializeAsync(app.Services);

app.Run();

