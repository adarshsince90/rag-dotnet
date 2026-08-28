# ADR-017: Character-Based Chunking Strategy

**Status:** Accepted
**Sprint:** 5A

## Context

Experiments with different chunking strategies showed that chunk size and overlap significantly impact retrieval quality.

## Decision

Use `CharacterChunkingStrategy` with:

- **ChunkSize**: 1000 characters
- **ChunkOverlap**: 200 characters

## Rationale

This configuration provided the best balance between retrieval quality and chunk count during experiments documented in the [Chunking Lab](../experiments/chunking-lab.md).

## Consequences

- Reliable retrieval for technical documentation
- ~629 chunks generated from 5 research papers
- Retrieval remains under 1 second

## Related

- [Sprint 05A](../sprint/Sprint-05A.md) — Chunking experiments
- [Chunking concept](../concepts/08-Chunking.md)
- [Chunking Lab](../experiments/chunking-lab.md) — Experimental results
