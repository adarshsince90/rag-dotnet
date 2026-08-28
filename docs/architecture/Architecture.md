# Architecture

## Overview

The application follows Clean Architecture with four layers that separate API concerns, application workflows, business contracts, and infrastructure implementations.

```
src/
├── RagDemo.Api              → HTTP endpoints, DI, middleware
├── RagDemo.Application      → Orchestration, services, prompt building
├── RagDemo.Domain           → Interfaces, models, contracts
├── RagDemo.Infrastructure   → Ollama, Groq, Qdrant, PDF, memory
└── RagDemo.Cli              → Interactive console client
```

- **Application** owns the workflow
- **Infrastructure** owns the implementation
- **Domain** owns the contracts
- **API** owns the transport

---

## Layer Responsibilities

### API Layer (RagDemo.Api)

- HTTP endpoints (Minimal APIs)
- Request/response handling
- OpenAPI / Scalar documentation
- Input validation
- Dependency injection configuration
- CORS, middleware, static files

The API layer does not contain business logic or infrastructure details.

### Application Layer (RagDemo.Application)

- RAG pipeline orchestration
- Prompt construction (`RagPromptBuilder`)
- Query classification
- Retrieval service coordination
- Embedding generation orchestration
- Evaluation service

### Domain Layer (RagDemo.Domain)

- Interface contracts for all infrastructure concerns
- Domain models (`DocumentChunk`, `ConversationTurn`, `RetrievalResult`, etc.)
- Evaluation models and contracts

### Infrastructure Layer (RagDemo.Infrastructure)

- LLM providers (Ollama, Groq)
- Embedding generation (Ollama / nomic-embed-text)
- Vector storage (Qdrant)
- PDF document extraction (PdfPig)
- Chunking strategies
- Conversation memory (in-memory)
- Evaluation dataset provider

---

## Interface → Implementation Map

| Domain Interface | Infrastructure Implementation | Purpose |
|-----------------|------------------------------|---------|
| `IChatCompletionService` | `OllamaChatCompletionService`, `GroqChatCompletionService` | LLM generation |
| `IRetriever` | `VectorRetriever` | Vector search |
| `IEmbeddingGenerator` | `OllamaEmbeddingGenerator` | Embedding generation |
| `IVectorStore` | `QdrantVectorStore` | Persistent vector storage |
| `IChunkProvider` | `PdfChunkProvider` | Document chunking |
| `IChunkingStrategy` | `CharacterChunkingStrategy` | Chunking algorithm |
| `IDocumentExtractor` | `PdfDocumentExtractor` | PDF text extraction |
| `IPromptBuilder` | `RagPromptBuilder` | Prompt construction |
| `IConversationMemory` | `InMemoryConversationMemory` | Conversation history |
| `IConversationQueryBuilder` | `ConversationQueryBuilder` | History-aware queries |
| `IRetrievalResultProcessor` | `RetrievalResultProcessor` | Ranking and filtering |
| `IChunkStore` | `InMemoryChunkStore` | In-memory chunk cache |
| `IEvaluationService` | `EvaluationService` | Automated benchmarking |
| `IEvaluationDatasetProvider` | `JsonEvaluationDatasetProvider` | Test dataset loading |

---

## Conversational RAG Pipeline

The primary pipeline used by the chat UI, CLI, and evaluation:

```
ConversationId + Question
        ↓
QueryClassifier
   ├── Greeting → Direct response (no retrieval)
   ├── Capabilities → Direct response (no retrieval)
   └── DocumentQuestion ↓
        ↓
Load Conversation History (last 4 turns)
        ↓
Build Retrieval Query (previous questions + current)
        ↓
VectorRetriever
   ├── Embed question (nomic-embed-text, 768d)
   ├── Qdrant search (top 10 candidates)
   └── RetrievalResultProcessor (filter ≥ 0.55, top 5)
        ↓
RagPromptBuilder
   ├── System instructions (grounded prompting)
   ├── Conversation history
   ├── Retrieved document context
   └── User question
        ↓
LLM (Ollama or Groq)
        ↓
SSE Streaming Response
        ↓
Store conversation turn in memory
```

---

## Ingestion Pipeline

Triggered via `POST /generate-embeddings`:

```
PDF Documents (data/raw/pdf/)
        ↓
PdfDocumentExtractor (PdfPig)
        ↓
CharacterChunkingStrategy (1000 chars, 200 overlap)
        ↓
OllamaEmbeddingGenerator (nomic-embed-text, 768d)
   └── Concurrent (8 parallel requests)
        ↓
QdrantVectorStore
   └── Upserts: vector + content + metadata (source, chunkIndex)
```

---

## Multi-Provider Architecture

The AI provider is selected at startup via configuration:

```
appsettings.json → Ai:DefaultProvider → "local" | "groq"
        ↓
AiServiceCollectionExtensions
   ├── "Ollama" → OllamaChatCompletionService
   └── "groq"  → GroqChatCompletionService
```

Both implement `IChatCompletionService` with sync and streaming support.

Embeddings always use Ollama locally (nomic-embed-text).

---

## Memory Architecture

Conversation memory and knowledge retrieval are independent concerns:

| Type | Storage | Purpose |
|------|---------|---------|
| **Conversation Memory** | `InMemoryConversationMemory` (ConcurrentDictionary) | User questions + assistant responses + timestamps |
| **Knowledge Memory** | Qdrant | Document chunks + embeddings + metadata |

The system combines both to generate context-aware responses.

---

## Evaluation Pipeline

```
Evaluation Dataset (12 questions, JSON)
        ↓
EvaluationService
        ↓
ConversationQuestionAnsweringService (same as production pipeline)
        ↓
Results: keyword coverage, source coverage, grounding accuracy, latency
```

---

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api-conversation/stream` | POST | Conversational RAG with SSE streaming |
| `/ask` | POST | Single-turn RAG |
| `/ask/stream` | POST | Single-turn RAG with streaming |
| `/retrieve` | POST | Retrieval only |
| `/generate-embeddings` | POST | Document ingestion |
| `/evaluation/run` | POST | Automated evaluation |
| `/health` | GET | Health check |

---

## Key ADRs

- [ADR-001](../adr/ADR-001-dependency-inversion.md) — Dependency inversion
- [ADR-012](../adr/ADR-012-prompt-separation.md) — Prompt construction separation
- [ADR-013](../adr/ADR-013-grounded-prompting.md) — Grounded prompting
- [ADR-020](../adr/ADR-020-qdrant-vector-storage.md) — Qdrant vector storage
- [ADR-021](../adr/ADR-021-conversational-memory.md) — Conversational memory