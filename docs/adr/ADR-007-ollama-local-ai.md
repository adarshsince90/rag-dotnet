# ADR-007: Local AI Infrastructure Uses Ollama

**Status:** Accepted
**Sprint:** 3

## Context

The project requires local AI infrastructure for embedding generation and chat completion without cloud API costs.

## Decision

Ollama will be used as the default provider for:

- Embedding Generation (nomic-embed-text)
- Chat Completion (gemma2:2b)

## Rationale

- Open source
- Runs locally
- Supports multiple models
- No API cost
- Compatible with assignment requirements

## Consequences

- Zero-cost local development
- Hardware-dependent performance
- Model quality limited by local compute

## Related

- [Sprint 03](../sprint/Sprint-03.md) — Embeddings and semantic retrieval
- [Sprint 04](../sprint/Sprint-04.md) — LLM integration
- [ADR-011](ADR-011-ollama-chat-completion.md) — Chat completion model selection
- [Embeddings concept](../concepts/03-Embeddings.md)
