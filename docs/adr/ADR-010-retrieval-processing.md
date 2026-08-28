# ADR-010: Centralized Retrieval Processing

**Status:** Accepted
**Sprint:** 2

## Context

Ranking, filtering, and Top-K selection need to be consistent across all retriever implementations (keyword, vector, hybrid).

## Decision

Ranking, filtering, and Top-K selection are handled by `RetrievalResultProcessor`.

This ensures consistent retrieval behavior regardless of which `IRetriever` implementation is used.

## Consequences

- Consistent ranking logic across retriever implementations
- Single place to tune retrieval parameters
- Retriever implementations focus on search, not filtering

## Related

- [Sprint 02](../sprint/Sprint-02.md) — Ranking and diagnostics
- [ADR-005](ADR-005-topk-retrieval.md) — Top-K retrieval
