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