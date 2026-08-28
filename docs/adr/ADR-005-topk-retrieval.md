# ADR-005: Top-K Retrieval

**Status:** Accepted
**Sprint:** 2

## Context

Modern RAG systems typically use multiple retrieved chunks to construct prompts. Returning Top-K results enables future LLM integration without additional retriever redesign.

## Decision

Retrievers should return ranked collections rather than a single result.

## Consequences

- Multiple chunks available for context construction
- Future LLM integration requires no retriever changes
- Configurable via `TopK` setting

## Related

- [Sprint 02](../sprint/Sprint-02.md) — Ranking and diagnostics
- [ADR-004](ADR-004-retrieval-metadata.md) — Retrieval metadata
- [ADR-010](ADR-010-retrieval-processing.md) — Centralized retrieval processing
