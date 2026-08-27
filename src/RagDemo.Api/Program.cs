using Scalar.AspNetCore;
using RagDemo.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services
	.AddOptionsConfiguration(builder.Configuration)
	.AddApplicationServices()
	.AddInfrastructureServices()
    .AddAiServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "web-ui",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5268")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("web-ui");

app.MapHealthChecks("/health");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapDiagnosticEndpoints();
app.MapRetrievalEndpoints();
app.MapGenerateEmbeddingsEndpoints();
app.MapQuestionEndpoints();
app.MapConversationEndpoints();
app.MapEvaluationEndpoints();

await StartupTasks.InitializeAsync(app.Services);

app.Run();

