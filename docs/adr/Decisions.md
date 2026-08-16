# ADR-001: Dependency Inversion

## Status

Accepted

## Decision

Application logic must depend on abstractions rather than concrete implementations.

External technologies must be hidden behind interfaces.

Examples:

- Ollama -> IChatModel
- Qdrant -> IVectorStore
- PdfPig -> IPdfExtractor

## Rationale

This allows infrastructure components to be replaced without modifying business logic.

## Consequences

Higher maintainability.
Better testability.
Reduced vendor lock-in.

## ADR-004

Title:
Retrieval Metadata Should Be Preserved

Decision:

Retrievers should return retrieval metadata such as score and ranking information rather than only the selected chunk.

Reason:

Retrieval quality must be measurable, explainable, and debuggable.

Future vector-based retrieval will also require similarity scores.

## ADR-005

Title

Retrieval Uses Top-K Results

Decision

Retrievers should return ranked collections rather than a single result.

Reason

Modern RAG systems typically use multiple retrieved chunks to construct prompts.

Returning Top-K results enables future LLM integration without additional retriever redesign.

## ADR-006

Title

Document Metadata Is Preserved

Decision

Every chunk should maintain source metadata.

Current metadata:

- Source
- ChunkIndex

Future metadata:

- Document Name
- Page Number
- Section

Reason

Supports traceability, debugging, citations, and source attribution.