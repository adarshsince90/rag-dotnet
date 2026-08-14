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