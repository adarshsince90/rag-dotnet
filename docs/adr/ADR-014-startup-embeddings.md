# ADR-014: Embeddings Generated At Startup

**Status:** Superseded by [ADR-020](ADR-020-qdrant-vector-storage.md)
**Sprint:** 3

## Context

Current dataset is small. In-memory retrieval requires embeddings to be available at startup.

## Decision

Embeddings are generated during application startup and stored in `InMemoryChunkStore`.

## Consequences

- Simple developer workflow
- No external dependencies for small datasets
- Slow startup with larger document sets (~10 min for 5 PDFs)

## Superseded

When Qdrant was introduced in Sprint 6 ([ADR-020](ADR-020-qdrant-vector-storage.md)), embeddings are now generated during document ingestion via the `/generate-embeddings` endpoint rather than at application startup.

## Related

- [Sprint 03](../sprint/Sprint-03.md) — Embeddings
- [Sprint 06](../sprint/Sprint-06.md) — Qdrant migration
- [ADR-009](ADR-009-inmemory-embeddings.md) — In-memory storage (also superseded)
- [ADR-020](ADR-020-qdrant-vector-storage.md) — Persistent vector storage
