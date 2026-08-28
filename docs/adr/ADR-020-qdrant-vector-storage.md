# ADR-020: Persistent Vector Storage (Qdrant)

**Status:** Accepted
**Sprint:** 6
**Supersedes:** [ADR-009](ADR-009-inmemory-embeddings.md), [ADR-014](ADR-014-startup-embeddings.md)

## Context

The application originally relied upon `InMemoryChunkStore`. Embeddings were regenerated during application startup.

As document volume increased, indexing five PDF documents required approximately ten minutes.

A persistent vector database was required.

## Decision

Introduce Qdrant as the primary vector storage engine.

Implementation:

- `IVectorStore` — Interface in Domain layer
- `QdrantVectorStore` — Qdrant implementation

Storage includes:

- Embedding vectors (768 dimensions)
- Chunk content
- Metadata (source, chunkIndex)

Vectors persist independently of application lifecycle.

## Consequences

### Benefits

- Persistent storage
- Faster startup (no re-indexing)
- Scalability
- Metadata support
- Future filtering support

### Tradeoffs

- Additional infrastructure dependency (Docker)
- Vector database operational complexity

## Future Opportunities

- Metadata filtering
- Collection statistics
- Hybrid retrieval
- Dynamic search limit
- Query classification
- Agentic retrieval

## Related

- [Sprint 06](../sprint/Sprint-06.md) — Qdrant integration
- [ADR-009](ADR-009-inmemory-embeddings.md) — Previous in-memory approach (superseded)
- [ADR-014](ADR-014-startup-embeddings.md) — Previous startup embeddings (superseded)
- [Vector Database concept](../concepts/09-vector-database.md)
