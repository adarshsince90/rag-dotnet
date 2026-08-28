# ADR-011: Ollama Chat Completion Model

**Status:** Accepted
**Sprint:** 4

## Context

The project needs a local chat completion model for RAG answer generation without API costs.

## Decision

Ollama is used for answer generation with model: `gemma2:2b`.

## Rationale

- Open source
- Runs locally
- No usage cost
- Satisfies assignment requirements

## Consequences

- Zero-cost generation
- Performance limited by local hardware
- Generation is the primary latency bottleneck (~20-40s)

## Related

- [Sprint 04](../sprint/Sprint-04.md) — LLM integration
- [ADR-007](ADR-007-ollama-local-ai.md) — Ollama as local AI
- [ADR-012](ADR-012-prompt-separation.md) — Prompt construction separation
