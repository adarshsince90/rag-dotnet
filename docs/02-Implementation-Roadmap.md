# Implementation Roadmap

Sprint-by-sprint evolution from foundation to a full conversational RAG system.

| Sprint | Title | Status | Key Deliverable |
|--------|-------|--------|----------------|
| [Sprint 00](sprint/Sprint-00.md) | Foundation | ✅ | Clean Architecture scaffold, core interfaces |
| [Sprint 01](sprint/Sprint-01.md) | Keyword Retrieval | ✅ | TF-IDF retriever, chunking, `KeywordRetriever` |
| [Sprint 02](sprint/Sprint-02.md) | Ranking & Diagnostics | ✅ | Top-K, scoring, `RetrievalResultProcessor` |
| [Sprint 03](sprint/Sprint-03.md) | Embeddings & Semantic Retrieval | ✅ | Ollama embeddings, cosine similarity, `VectorRetrieverInMemory` |
| [Sprint 04](sprint/Sprint-04.md) | LLM Integration | ✅ | RAG pipeline, grounded prompting, `QuestionAnsweringService` |
| [Sprint 05A](sprint/Sprint-05A.md) | Chunking Experiments | ✅ | `CharacterChunkingStrategy` (1000/200) |
| [Sprint 05B](sprint/Sprint-05B.md) | PDF Support | ✅ | PdfPig extraction, `PdfChunkProvider` |
| [Sprint 06](sprint/Sprint-06.md) | Persistent Vector Storage | ✅ | Qdrant integration, `QdrantVectorStore` |
| [Sprint 07A](sprint/Sprint-07A.md) | Streaming Responses | ✅ | SSE, `IAsyncEnumerable`, `/ask/stream` |
| [Sprint 07B](sprint/Sprint-07B.md) | Conversational Memory | ✅ | `InMemoryConversationMemory`, history-aware retrieval |
| [Sprint 08](sprint/Sprint-08.md) | Evaluation & Testing | ✅ | 12-question benchmark, `EvaluationService` |
| [Sprint 09A](sprint/Sprint-09A.md) | Production Hardening | ✅ | Refactoring, error handling |
| [Sprint 10](sprint/Sprint-10.md) | Interactive Console Client | ✅ | `RagDemo.Cli` with streaming |
| [Sprint 11](sprint/Sprint-11.md) | Multi-Provider AI | ✅ | Groq cloud provider, `GroqChatCompletionService` |
| [Sprint 12](sprint/Sprint-12.md) | Browser Chat UI | ✅ | Static HTML/CSS/JS chat interface |

## Key Milestones

- **Sprint 4**: First end-to-end RAG pipeline working
- **Sprint 6**: Persistent storage — no more startup re-indexing
- **Sprint 7B**: Conversational memory — multi-turn interactions
- **Sprint 11**: Cloud provider — API-based LLM as alternative to local
- **Sprint 12**: Browser UI — visual chat interface

## Future

See [Future Directions](03-Future-Directions.md) for planned enhancements.