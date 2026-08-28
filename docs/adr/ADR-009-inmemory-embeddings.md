# ADR-009: In-Memory Embedding Storage

**Status:** Superseded by [ADR-020](ADR-020-qdrant-vector-storage.md)
**Sprint:** 3

## Context

Dataset size is small and no vector database is currently required.

## Decision

Generated embeddings are stored using `InMemoryChunkStore`.

## Consequences

- Simple implementation
- Fast retrieval for small datasets
- Embeddings lost on restart
- Not scalable

## Superseded

This decision was superseded in Sprint 6 by [ADR-020](ADR-020-qdrant-vector-storage.md), which introduced Qdrant as persistent vector storage.

## Related

- [Sprint 03](../sprint/Sprint-03.md) — Embeddings
- [Sprint 06](../sprint/Sprint-06.md) — Qdrant migration
- [ADR-020](ADR-020-qdrant-vector-storage.md) — Persistent vector storage
