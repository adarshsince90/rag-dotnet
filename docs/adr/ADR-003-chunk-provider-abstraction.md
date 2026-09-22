# ADR-003: Chunk Provider Abstraction for Document Ingestion

**Status:** Accepted (Extended by [ADR-019](ADR-019-pdf-support.md))  
**Sprint:** 1

## Context

The retrieval system requires text chunks to index and search. Raw knowledge sources can arrive in various formats (plain text files, markdown, PDFs, database dumps). Tightly coupling document extraction and chunking directly to retrieval services would violate Single Responsibility and clean architecture boundaries.

## Decision

Introduce an `IChunkProvider` abstraction in the Domain/Application layer:

```csharp
public interface IChunkProvider
{
    Task<IReadOnlyList<DocumentChunk>> GetChunksAsync(CancellationToken cancellationToken = default);
}
```

Implement the initial text file provider (`TextFileChunkProvider`) in the Infrastructure layer to load and slice documents into `DocumentChunk` records.

## Rationale

- Decouples document file I/O, format parsing, and chunking strategy from search and vector generation.
- Allows downstream services (retrievers, embedding generators, evaluators) to depend only on `IChunkProvider`.
- Enables straightforward replacement or composition when adding new document formats (e.g. PDF support in ADR-019).

## Consequences

- Clear separation between raw document preparation and retrieval orchestration.
- Reusable pipeline: when PDF extraction was introduced in Sprint 5B, `PdfChunkProvider` dropped in without modifying any retriever logic.
- Simplified unit testing using in-memory mock chunk providers.

## Related

- [Sprint 01](../sprint/Sprint-01.md) — Retrieval fundamentals
- [ADR-001](ADR-001-dependency-inversion.md) — Dependency inversion
- [ADR-006](ADR-006-document-metadata.md) — Document metadata preservation
- [ADR-019](ADR-019-pdf-support.md) — PDF document support
