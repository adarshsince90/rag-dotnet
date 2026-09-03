# RagDemo

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)](docs/architecture/Architecture.md)
[![Vector DB](https://img.shields.io/badge/Vector%20DB-Qdrant-red)](https://qdrant.tech/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A Conversational Retrieval-Augmented Generation (RAG) chatbot built from first principles in .NET 10, following Clean Architecture.

## Tech Stack

- **.NET 10** — Clean Architecture with dependency inversion
- **Ollama** — Local LLM (gemma2:2b) and embeddings (nomic-embed-text)
- **Groq** — Cloud LLM provider (optional)
- **Qdrant** — Persistent vector database
- **PdfPig** — PDF text extraction
- **SSE** — Server-Sent Events for streaming responses

## Features

- PDF document ingestion and chunking
- Semantic vector search (768-dimensional embeddings)
- Conversational memory (last 4 turns)
- Streaming responses via SSE
- Multi-provider AI support (Ollama / Groq)
- Grounded prompting (hallucination reduction)
- Retrieval, prompt, and generation diagnostics
- Automated evaluation framework (12-question benchmark)
- Browser chat UI, CLI client, and API

## Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- [Ollama](https://ollama.com/) (for local models)

### 1. Start Infrastructure

```bash
cd infrastructure
docker compose up -d
```

### 2. Pull Models

```bash
ollama pull nomic-embed-text
ollama pull gemma2:2b
```

### 3. Run the API

```bash
dotnet run --project src/RagDemo.Api
```

### 4. Generate Embeddings (First Time)

```bash
curl -X POST http://localhost:5000/generate-embeddings
```

### 5. Start Chatting

- **Browser UI**: Open `http://localhost:5000/index.html`
- **API docs**: Open `http://localhost:5000/scalar`
- **CLI client**: `dotnet run --project src/RagDemo.Cli`

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/conversation/stream` | POST | Conversational RAG with streaming (recommended) |
| `/api-conversation/stream` | POST | Conversational RAG with streaming from API |
| `/ask` | POST | Single-turn question answering |
| `/ask/stream` | POST | Single-turn with streaming |
| `/retrieve` | POST | Retrieval only (no generation) |
| `/generate-embeddings` | POST | Trigger document ingestion |
| `/evaluation/run` | POST | Run evaluation benchmark |
| `/health` | GET | Health check |

## Architecture

The project follows Clean Architecture with four layers:

```
src/
├── RagDemo.Api              → HTTP endpoints, DI configuration
├── RagDemo.Application      → Orchestration, prompt building, services
├── RagDemo.Domain           → Interfaces, models, contracts
├── RagDemo.Infrastructure   → Ollama, Groq, Qdrant, PDF, memory
└── RagDemo.Cli              → Interactive console client
```

See [docs/architecture/Architecture.md](docs/architecture/Architecture.md) for the full architecture documentation.

## AI Providers

The application supports multiple AI providers through configuration:

| Provider | Type | Config Key |
|----------|------|-----------|
| Ollama (local) | Embedding + Chat | `local` |
| Groq (cloud) | Chat only | `groq` |

Switch providers in `appsettings.json`:

```json
{
  "Ai": {
    "DefaultProvider": "local"
  }
}
```

### Groq Setup

```bash
dotnet user-secrets set \
  "Ai:Providers:groq:ApiKey" \
  "gsk_xxxxxxxxx" \
  --project src/RagDemo.Api
```

## CLI Client

Start the API, then in a separate terminal:

```bash
dotnet run --project src/RagDemo.Cli
```

Commands: `/new` (new conversation), `/exit` (quit).

## Documentation

Full documentation is available in the [docs/](docs/README.md) directory:

- [Architecture](docs/architecture/Architecture.md) — System design and component overview
- [ADRs](docs/adr/README.md) — Architecture Decision Records
- [Sprint History](docs/sprint/) — 13 sprints documenting the build journey
- [Concepts](docs/concepts/) — RAG concept explanations
- [Setup](docs/setup/Local-Development.md) — Local development guide
- [Evaluation](data/evaluation/README.md) — Evaluation framework and results
- [Vision](docs/00-Vision.md) — Project vision and learning philosophy

## Project Evolution

This project was built incrementally through 13 sprints, starting from keyword retrieval and evolving to a full conversational RAG system. See [docs/02-Implementation-Roadmap.md](docs/02-Implementation-Roadmap.md) for the complete sprint history.

## License

This project is licensed under the [MIT License](LICENSE).
