- IDocumentExtractor
- PdfDocumentExtractor
- PdfChunkProvider
- PdfPig integration
- Multi-document PDF ingestion

Existing components reused:

- IChunkingStrategy
- GenerateEmbeddingsService
- InMemoryChunkStore
- VectorRetriever

## Observations

PDF extraction quality was good using PdfPig.

Most implementation effort was not extraction,
but handling embedding generation on larger datasets.

Embedding generation completed in:

559054 ms (~9.3 minutes)

Observation:

## Indexing Performance

Embedding generation became the dominant cost in the pipeline.

Approximate costs:

- Extraction: negligible
- Chunking: negligible
- Embedding generation: dominant
- Retrieval: milliseconds


## Key Findings

### Finding 1

PDF ingestion successfully integrated into the existing RAG architecture with minimal changes.

### Finding 2

The existing chunking abstractions required no modification, validating the original design.

### Finding 3

Retrieval remained fast even with a corpus of more than 600 chunks.

Typical retrieval latency:

- 500ms to 1000ms

### Finding 4

LLM generation remains the primary user-facing bottleneck.

Observed example:

- Retrieval: ~864ms
- Generation: ~40.6s

### Finding 5

Embedding generation became the dominant indexing cost.

Observed indexing time:

- ~559 seconds (9.3 minutes)

This revealed the need for persistent vector storage rather than generating embeddings on every application startup.

### Finding 6

PDF extraction occasionally removes whitespace between words.

Example:

attention mechanisms
↓
attentionmechanisms

Retrieval quality remained acceptable despite this issue.
---

## Related

- **ADRs**: [ADR-019 PDF Support](../adr/ADR-019-pdf-support.md)
- **Previous Sprint**: [Sprint 05A — Chunking Experiments](Sprint-05A.md)
- **Next Sprint**: [Sprint 06 — Vector Storage](Sprint-06.md)
