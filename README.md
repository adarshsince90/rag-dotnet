docs/
│
├── architecture/
│   └── architecture-overview.md
│
├── evaluation/
│   ├── README.md
│   ├── evaluation-methodology.md
│   └── evaluation-results-v2.md
│
├── sprints/
│   ├── sprint-1.md
│   ├── sprint-2.md
│   └── sprint-8b-summary.md
│
└── setup/
    └── local-development.md

# RagDemo

A Conversational Retrieval-Augmented Generation (RAG) chatbot built with:

- .NET 10
- Clean Architecture
- Ollama
- Qdrant
- PDF Document Processing
- Conversational Memory
- Streaming Responses
- Evaluation Framework

## Prerequisites

- .NET 10 SDK
- Docker
- Ollama
- Qdrant


## API Documentation

After application startup:

http://localhost:5000/scalar

The Scalar UI provides all available API endpoints and request models.


## Recommended Endpoint

For normal chatbot usage:

POST /conversation/stream

This endpoint provides:

- Conversational memory
- Streaming responses
- Retrieval diagnostics
- Prompt diagnostics
- Generation diagnostics



## Evaluation Endpoint

POST /evaluation/run


## Console Chat Client

Start the API:

```bash
dotnet run --project src/RagDemo.Api
```

In a separate terminal:

```bash
dotnet run --project src/RagDemo.Cli
```

Available commands:

```text
/new   Start a new conversation
/exit  Exit the application
```

Example:

```text
You: What is self-attention?

Assistant:
Self-attention is an attention mechanism...

Retrieval
--------------------------------
Time           : 78 ms
Chunks         : 5
Highest Score  : 0.72

Sources:
 • attention-is-what-you-need.pdf
```