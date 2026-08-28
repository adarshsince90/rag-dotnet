# ADR-004: Retrieval Metadata Preservation

**Status:** Accepted
**Sprint:** 2

## Context

Retrieval quality must be measurable, explainable, and debuggable. Future vector-based retrieval will also require similarity scores.

## Decision

Retrievers should return retrieval metadata such as score and ranking information rather than only the selected chunk.

## Consequences

- Retrieval quality becomes measurable
- Debugging retrieval issues is straightforward
- Similarity scores available for all retrieval strategies

## Related

- [Sprint 02](../sprint/Sprint-02.md) — Ranking and diagnostics
- [ADR-005](ADR-005-topk-retrieval.md) — Top-K retrieval
- [Ranking concept](../concepts/02-Ranking.md)
