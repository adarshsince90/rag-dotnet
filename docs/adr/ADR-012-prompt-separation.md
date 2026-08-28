# ADR-012: Prompt Construction Separated From LLM Provider

**Status:** Accepted
**Sprint:** 4

## Context

Prompt construction is application logic, not infrastructure logic. It should be independent of the LLM provider being used.

## Decision

Prompt generation is implemented via `IPromptBuilder` in the Application layer.

The current implementation, `RagPromptBuilder`, constructs grounded prompts with:

- System instructions
- Retrieved document context
- Conversation history (for conversational mode)
- User question

## Rationale

- Provider independent
- Reusable across different LLM backends
- Easier experimentation with prompt strategies

## Consequences

- Prompt changes don't require infrastructure changes
- Multiple prompt strategies can be tested independently
- Clear separation between "what to ask" and "how to ask"

## Related

- [Sprint 04](../sprint/Sprint-04.md) — LLM integration
- [ADR-013](ADR-013-grounded-prompting.md) — Grounded prompting strategy
- [Prompting concept](../concepts/07-Prompting.md)
