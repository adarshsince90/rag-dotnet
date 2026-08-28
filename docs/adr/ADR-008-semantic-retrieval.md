# ADR-008: Semantic Retrieval Uses Embeddings

**Status:** Accepted
**Sprint:** 3

## Context

Exact keyword matching has limited retrieval quality. Semantic similarity provides better results for natural language questions.

## Decision

Document chunks are converted into embeddings using an embedding model (nomic-embed-text, 768 dimensions).

Retrieval uses cosine similarity between question embeddings and document embeddings.

## Consequences

- Better retrieval quality for natural language queries
- Tolerates minor spelling errors
- Requires embedding generation infrastructure
- Requires vector storage

## Related

- [Sprint 03](../sprint/Sprint-03.md) — Embeddings and semantic retrieval
- [ADR-007](ADR-007-ollama-local-ai.md) — Ollama as embedding provider
- [Embeddings concept](../concepts/03-Embeddings.md)
- [Cosine Similarity concept](../concepts/05-CosineSimilarity.md)
- [Vectors concept](../concepts/04-Vectors.md)
