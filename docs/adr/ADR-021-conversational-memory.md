# ADR-021: Conversational Memory

**Status:** Accepted
**Sprint:** 7B

## Context

The application supported only single-turn interactions. Follow-up questions lacked historical context.

## Decision

Introduce:

- `IConversationMemory` — Interface for conversation storage
- `InMemoryConversationMemory` — In-memory implementation using `ConcurrentDictionary`

Conversation history is injected into prompts. History-aware retrieval is implemented using previous questions combined with the current question, instead of introducing query rewriting.

Maximum history: 4 turns (configurable).

## Consequences

### Benefits

- Follow-up question support
- Conversation continuity
- Minimal architecture changes

### Tradeoffs

- Larger prompts
- Increased generation time
- Memory lost on restart

## Future Opportunities

- Persistent memory
- Semantic memory
- Query rewriting
- Memory summarization

## Related

- [Sprint 07B](../sprint/Sprint-07B.md) — Conversational memory
- [Conversational Memory concept](../concepts/11-Conversational-memory.md)
- [ADR-012](ADR-012-prompt-separation.md) — Memory-aware prompt builder
