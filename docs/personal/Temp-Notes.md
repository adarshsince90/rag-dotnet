# Architecture Reference & Retrieval Heuristics

## Project Structure & Component Map

```text
src/
├── RagDemo.Api/
│   ├── Endpoints/
│   │   ├── ConversationEndpoints.cs    # /conversation/stream, /api-conversation/stream
│   │   ├── AskEndpoints.cs             # /ask, /ask/stream
│   │   ├── RetrievalEndpoints.cs       # /retrieve
│   │   ├── IngestionEndpoints.cs       # /generate-embeddings
│   │   ├── EvaluationEndpoints.cs      # /evaluation/run
│   │   └── HealthEndpoints.cs          # /health
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs # DI registrations & provider resolution
│   └── wwwroot/                        # Browser chat UI (index.html, styles, app.js)
│
├── RagDemo.Application/
│   ├── Services/
│   │   ├── ConversationQuestionAnsweringService.cs
│   │   ├── QuestionAnsweringService.cs
│   │   ├── RetrievalService.cs
│   │   ├── GenerateEmbeddingsService.cs
│   │   └── EvaluationService.cs
│   └── Prompts/
│       ├── RagPromptBuilder.cs
│       └── ConversationQueryBuilder.cs
│
├── RagDemo.Domain/
│   ├── Interfaces/
│   │   ├── IChatCompletionService.cs
│   │   ├── IEmbeddingGenerator.cs
│   │   ├── IVectorStore.cs
│   │   ├── IRetriever.cs
│   │   ├── IConversationMemory.cs
│   │   ├── IDocumentExtractor.cs
│   │   └── IChunkingStrategy.cs
│   └── Models/
│       ├── DocumentChunk.cs
│       ├── ScoredChunk.cs
│       └── ChatMessage.cs
│
├── RagDemo.Infrastructure/
│   ├── AI/
│   │   ├── Ollama/ (Chat & Embeddings)
│   │   └── Groq/ (Cloud Chat)
│   ├── VectorStore/
│   │   ├── QdrantVectorStore.cs
│   │   └── InMemoryVectorStore.cs
│   ├── Ingestion/
│   │   ├── PdfDocumentExtractor.cs
│   │   └── CharacterChunkingStrategy.cs
│   └── Memory/
│       └── InMemoryConversationMemory.cs
│
└── RagDemo.Cli/
    └── Program.cs                      # Interactive console terminal client
```

---

## API Endpoints Summary

| Endpoint | Method | Payload / Response | Description |
|---|---|---|---|
| `/conversation/stream` | POST | `{ "conversationId": "...", "question": "..." }` ➔ SSE | Recommended multi-turn conversational streaming |
| `/api-conversation/stream`| POST | `{ "conversationId": "...", "question": "..." }` ➔ SSE | API-optimized conversational streaming |
| `/ask` | POST | `{ "question": "..." }` ➔ JSON `{ "answer", "diagnostics" }` | Single-turn non-streaming RAG |
| `/ask/stream` | POST | `{ "question": "..." }` ➔ SSE | Single-turn streaming RAG |
| `/retrieve` | POST | `{ "query": "...", "topK": 3 }` ➔ JSON `{ "chunks" }` | Pure retrieval without LLM generation |
| `/generate-embeddings` | POST | Trigger ingestion pipeline | Ingests PDFs from `data/Raw`, chunks, and upserts to Qdrant |
| `/evaluation/run` | POST | Runs 12-question benchmark suite | Returns JSON metrics for relevance, accuracy, and latency |
| `/health` | GET | `200 OK` | Service and dependency health status |

---

## Production Retrieval Signals

Instead of relying solely on a single static similarity threshold (e.g. `score > 0.70`), the engine evaluates composite retrieval signals:

1. **Signal 1 — Absolute Similarity Score**: Cosine similarity score of Top-1 chunk.
2. **Signal 2 — Top-1 vs Top-2 Delta**: Difference in similarity score between first and second ranked items.
3. **Signal 3 — Rank-1 vs Median Delta**: Score distance separating top match from background noise.
4. **Signal 4 — Exact Entity / Keyword Matching**: Hybrid reinforcement for exact dates, entity names, and acronyms.

---

## Sample Query Similarity Benchmark

| Query | Top Match Score | Categorization | Outcome |
|---|---|---|---|
| *Where are headquarters?* | `0.82` | In-Domain / High Confidence | Context retrieved & grounded answer generated |
| *Company location* | `0.79` | In-Domain / Semantic Match | Context retrieved & grounded answer generated |
| *Founded in?* | `0.88` | In-Domain / High Confidence | Context retrieved & grounded answer generated |
| *Google CEO* | `0.50` | Out of Domain / Low Relevance | Fallback rejection / Hallucination prevention |
| *Weather today* | `0.44` | Out of Domain / Unrelated | Fallback rejection / Hallucination prevention |
| *Capital of France* | `0.42` | Out of Domain / Unrelated | Fallback rejection / Hallucination prevention |