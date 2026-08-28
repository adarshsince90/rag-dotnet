# Local Development Setup

## Prerequisites

| Requirement | Version | Purpose |
|------------|---------|---------|
| .NET SDK | 10.0+ | Application runtime |
| Docker | Latest | Ollama + Qdrant containers |
| Ollama | Latest | Local LLM and embeddings |

## 1. Start Infrastructure

```bash
cd infrastructure
docker compose up -d
```

This starts:

| Service | Port | Purpose |
|---------|------|---------|
| Ollama | 11434 | LLM and embedding generation |
| Qdrant | 6333 (REST), 6334 (gRPC) | Vector database |

## 2. Pull Required Models

```bash
ollama pull nomic-embed-text
ollama pull gemma2:2b
```

### Verify Models

```bash
ollama list
```

Expected output should include:

- `nomic-embed-text` — Embedding model (768 dimensions)
- `gemma2:2b` — Chat completion model

### Verify Ollama

Open http://localhost:11434 — should return "Ollama is running".

## 3. Run the API

```bash
dotnet run --project src/RagDemo.Api
```

The API starts on `http://localhost:5000`.

### Verify API

- Health check: http://localhost:5000/health
- API docs: http://localhost:5000/scalar

## 4. Generate Embeddings (First Time Only)

Before asking questions, the PDF documents need to be ingested and embedded:

```bash
curl -X POST http://localhost:5000/generate-embeddings
```

This will:

1. Read PDF files from `data/raw/pdf/`
2. Extract text using PdfPig
3. Chunk text (1000 chars, 200 overlap)
4. Generate embeddings via Ollama (nomic-embed-text)
5. Store vectors in Qdrant

> **Note:** First-time embedding generation takes several minutes depending on hardware and document count.

## 5. Start Chatting

### Browser UI

Open http://localhost:5000/index.html

### CLI Client

In a separate terminal:

```bash
dotnet run --project src/RagDemo.Cli
```

Commands: `/new` (new conversation), `/exit` (quit).

### API (Scalar)

Open http://localhost:5000/scalar and use the `POST /api-conversation/stream` endpoint.

## Groq Cloud Provider (Optional)

To use Groq instead of local Ollama for chat completion:

### 1. Set API Key

```bash
dotnet user-secrets set "Ai:Providers:groq:ApiKey" "gsk_xxxxxxxxx" --project src/RagDemo.Api
```

### 2. Switch Provider

In `src/RagDemo.Api/appsettings.json`:

```json
{
  "Ai": {
    "DefaultProvider": "groq"
  }
}
```

> **Note:** Embeddings always use local Ollama regardless of the chat provider setting.

## Configuration Reference

Key settings in `src/RagDemo.Api/appsettings.json`:

| Setting | Default | Description |
|---------|---------|-------------|
| `Retrieval:TopK` | 5 | Maximum chunks returned |
| `Retrieval:MinimumSimilarity` | 0.55 | Minimum cosine similarity threshold |
| `Retrieval:SearchLimit` | 10 | Qdrant search candidates |
| `Chunking:ChunkSize` | 1000 | Characters per chunk |
| `Chunking:Overlap` | 200 | Overlap between chunks |
| `Conversation:MaxHistoryTurns` | 4 | Conversation history depth |
| `Qdrant:CollectionName` | ragdemo-documents | Qdrant collection name |
| `Qdrant:VectorSize` | 768 | Embedding dimensions |