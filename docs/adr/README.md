# Architecture Decision Records

This directory contains the Architecture Decision Records (ADRs) for the RagDemo project. Each ADR documents a significant technical decision made during development.

## Index

| ADR | Title | Status | Sprint |
|-----|-------|--------|--------|
| [ADR-001](ADR-001-dependency-inversion.md) | Dependency Inversion | Accepted | 0 |
| [ADR-004](ADR-004-retrieval-metadata.md) | Retrieval Metadata Preservation | Accepted | 2 |
| [ADR-005](ADR-005-topk-retrieval.md) | Top-K Retrieval | Accepted | 2 |
| [ADR-006](ADR-006-document-metadata.md) | Document Metadata Preservation | Accepted | 1 |
| [ADR-007](ADR-007-ollama-local-ai.md) | Local AI Uses Ollama | Accepted | 3 |
| [ADR-008](ADR-008-semantic-retrieval.md) | Semantic Retrieval Uses Embeddings | Accepted | 3 |
| [ADR-009](ADR-009-inmemory-embeddings.md) | In-Memory Embedding Storage | Superseded by ADR-020 | 3 |
| [ADR-010](ADR-010-retrieval-processing.md) | Centralized Retrieval Processing | Accepted | 2 |
| [ADR-011](ADR-011-ollama-chat-completion.md) | Ollama Chat Completion Model | Accepted | 4 |
| [ADR-012](ADR-012-prompt-separation.md) | Prompt Construction Separation | Accepted | 4 |
| [ADR-013](ADR-013-grounded-prompting.md) | Grounded Prompting | Accepted | 4 |
| [ADR-014](ADR-014-startup-embeddings.md) | Embeddings Generated At Startup | Superseded by ADR-020 | 3 |
| [ADR-017](ADR-017-character-chunking.md) | Character-Based Chunking | Accepted | 5A |
| [ADR-019](ADR-019-pdf-support.md) | PDF Document Support | Accepted | 5B |
| [ADR-020](ADR-020-qdrant-vector-storage.md) | Persistent Vector Storage (Qdrant) | Accepted | 6 |
| [ADR-021](ADR-021-conversational-memory.md) | Conversational Memory | Accepted | 7B |

## Numbering Gaps

ADR numbers 002, 003, 015, 016, and 018 were duplicates of other ADRs created during the iterative development process. Their content has been consolidated:

- ADR-015 → merged into [ADR-012](ADR-012-prompt-separation.md) (Prompt Builder Abstraction)
- ADR-016 → merged into [ADR-014](ADR-014-startup-embeddings.md) (Startup Embeddings)
- ADR-018 → merged into [ADR-013](ADR-013-grounded-prompting.md) (Grounded Prompting)

## Format

Each ADR follows a consistent structure:

```
# ADR-NNN: Title
Status: Accepted | Superseded by ADR-XXX
Sprint: N
## Context
## Decision
## Consequences
## Related (cross-links)
```
