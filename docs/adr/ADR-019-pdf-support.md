# ADR-019: PDF Document Support

**Status:** Accepted
**Sprint:** 5B

## Context

The original implementation operated on text files only. The assignment requirements and future roadmap require support for PDF-based knowledge sources.

## Decision

Introduce:

- `IDocumentExtractor` — Interface for text extraction
- `PdfDocumentExtractor` — PDF text extraction using PdfPig
- `PdfChunkProvider` — Produces chunks from PDF documents

`PdfChunkProvider` remains responsible for producing chunks. `PdfDocumentExtractor` is responsible solely for PDF text extraction.

## Consequences

### Benefits

- Separation of extraction from chunking
- Reuse of existing chunking strategies
- Reuse of embedding generation pipeline
- Reuse of retrieval pipeline

### Tradeoffs

- Initial indexing time increased significantly due to embedding generation
- PDF extraction occasionally loses whitespace (e.g., "attentionmechanisms")

### Future Work

- Persistent vector database (addressed in [ADR-020](ADR-020-qdrant-vector-storage.md))
- Asynchronous document indexing
- Support for additional document formats

## Related

- [Sprint 05B](../sprint/Sprint-05B.md) — PDF support
- [ADR-017](ADR-017-character-chunking.md) — Chunking strategy
- [ADR-020](ADR-020-qdrant-vector-storage.md) — Persistent storage (motivated by indexing cost)
