# ADR-001: Dependency Inversion

**Status:** Accepted
**Sprint:** 0

## Context

The application needs to integrate multiple external technologies (Ollama, Qdrant, PdfPig) while remaining testable and maintainable.

## Decision

Application logic must depend on abstractions rather than concrete implementations.

External technologies must be hidden behind interfaces.

Examples:

- Ollama → `IChatCompletionService`
- Qdrant → `IVectorStore`
- PdfPig → `IDocumentExtractor`

## Rationale

This allows infrastructure components to be replaced without modifying business logic.

## Consequences

- Higher maintainability
- Better testability
- Reduced vendor lock-in

## Related

- [Sprint 00](../sprint/Sprint-00.md) — Foundation architecture
- [Architecture](../architecture/Architecture.md) — Layer responsibilities
