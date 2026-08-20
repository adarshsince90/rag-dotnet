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

## ADR-007

Title

Local AI Infrastructure Uses Ollama

Decision

Ollama will be used as the default provider for:

- Embedding Generation
- Chat Completion

Reason

- Open Source
- Runs Locally
- Supports Multiple Models
- No API Cost
- Compatible With Assignment Requirements

## ADR-008

Title

Semantic Retrieval Uses Embeddings

Decision

Document chunks are converted into embeddings using an embedding model.

Reason

Semantic similarity provides better retrieval quality than exact keyword matching.

## ADR-009

Title

Embeddings Stored In Memory

Decision

Generated embeddings are stored using InMemoryChunkStore.

Reason

Dataset size is small and no vector database is currently required.

## ADR-010

Title

Retrieval Processing Centralized

Decision

Ranking, filtering and Top-K selection are handled by RetrievalResultProcessor.

Reason

Ensures consistent retrieval behavior across retriever implementations.

# ADR-011

Title

Local Chat Completion Uses Ollama

Status

Accepted

Decision

Ollama is used for answer generation.

Model:

gemma2:2b

Reason

- Open Source
- Runs Locally
- No Usage Cost
- Satisfies Assignment Requirements


# ADR-012

Title

Prompt Construction Separated From LLM Provider

Status

Accepted

Decision

Prompt generation is implemented via:

IPromptBuilder

Reason

Prompt construction is application logic, not infrastructure logic.

Benefits

- Provider independent
- Reusable
- Easier experimentation

# ADR-013

Title

LLMs Must Use Retrieved Context

Status

Accepted

Decision

Prompts instruct the model to answer only from retrieved context.

Reason

Reduces hallucination risk and keeps answers grounded in project documents.

# ADR-014

Title

Embeddings Generated At Startup

Decision

Embeddings are generated during application startup
and stored in InMemoryChunkStore.

Reason

Current dataset is small.
In-memory retrieval requires embeddings to be available.

Future

When a persistent vector database is introduced,
embeddings will be generated during document ingestion
rather than application startup.

# ADR-015

Title

Prompt Builder Abstraction

Decision

Prompt generation is separated from LLM infrastructure.

Reason

Prompting is application logic rather than provider logic.


# ADR-016

Title

Embeddings Initialized During Startup

Decision

Embeddings are generated during startup and stored in-memory.

Reason

Small dataset size and simplified developer workflow.

Future

Vector DB will replace startup embedding generation.


# ADR-017

Title

Character Based Chunking

Decision

Use CharacterChunkingStrategy.

Configuration:

ChunkSize = 1000
ChunkOverlap = 200

Reason

Provided best balance between retrieval quality and chunk count during experiments.


# ADR-018

Title

RAG Uses Grounded Prompting

Decision

LLM responses must be generated only from retrieved context.

Reason

Reduce hallucinations and improve reliability.


# ADR-019 : PDF Support

## Status

Accepted

## Context

The original implementation operated on text files only.

The assignment requirements and future roadmap require support
for PDF-based knowledge sources.

## Decision

Introduce:

- IDocumentExtractor
- PdfDocumentExtractor
- PdfChunkProvider

PdfChunkProvider remains responsible for producing chunks.

PdfDocumentExtractor is responsible solely for PDF text extraction.

## Consequences

Benefits:

- Separation of extraction from chunking.
- Reuse of existing chunking strategies.
- Reuse of embedding generation pipeline.
- Reuse of retrieval pipeline.

Tradeoffs:

- Initial indexing time increased significantly due to embedding generation.

Future work:

- Persistent vector database.
- Asynchronous document indexing.
- Support for additional document formats.