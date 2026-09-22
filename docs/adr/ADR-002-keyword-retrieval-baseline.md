# ADR-002: Keyword-Based Retrieval Baseline

**Status:** Superseded by [ADR-008](ADR-008-semantic-retrieval.md)  
**Sprint:** 1

## Context

Before introducing deep learning embedding models, vector databases, or complex language model generation, the system needed an initial retrieval mechanism to validate the end-to-end pipeline, contracts, and scoring mechanisms.

## Decision

Implement an initial lexical keyword-matching retriever (`KeywordRetriever`) implementing `IRetriever`.

The retriever performs:
1. Question text normalization (lowercase, punctuation removal).
2. Stop-word filtering (ignoring low-value words like "what", "where", "is", "the").
3. Term frequency keyword overlap scoring against available chunks.

## Rationale

- Establishes working vertical slice from API to retriever with minimal dependencies.
- Sets a clear, explainable baseline for future retrieval comparisons.
- Separates retrieval contracts from generation contracts early in the lifecycle.

## Consequences

- Successfully validates the application flow and dependency inversion pattern.
- Severe semantic limitations: fails on synonyms (e.g., "headquarters" vs "based"), typos, or conceptual paraphrasing.
- Justifies the migration to dense vector embeddings and semantic search in ADR-008.

## Related

- [Sprint 01](../sprint/Sprint-01.md) — Retrieval fundamentals
- [ADR-001](ADR-001-dependency-inversion.md) — Dependency inversion
- [ADR-008](ADR-008-semantic-retrieval.md) — Semantic retrieval uses embeddings
