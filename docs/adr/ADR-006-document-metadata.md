# ADR-006: Document Metadata Preservation

**Status:** Accepted
**Sprint:** 1

## Context

Traceability, debugging, citations, and source attribution require chunks to carry metadata about their origin.

## Decision

Every chunk should maintain source metadata.

Current metadata:

- Source (file name)
- ChunkIndex

Future metadata:

- Document Name
- Page Number
- Section

## Consequences

- Source attribution in responses
- Debugging which document a chunk came from
- Foundation for future citation features

## Related

- [Sprint 01](../sprint/Sprint-01.md) — Retrieval fundamentals
- [ADR-004](ADR-004-retrieval-metadata.md) — Retrieval metadata
