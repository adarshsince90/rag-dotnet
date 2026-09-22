## Sprint 7B - Introduce conversational memory and enhance multi-turn interaction capabilities

Enable follow-up questions through conversational memory while keeping the existing RAG architecture unchanged.

Without memory, each question was processed independently.

Example:

Q1:
What is attention?

Q2:
How does it help transformers?

The system had no understanding that:

"it" = attention

which reduced conversational quality.

Introduce a separate conversational flow.

Components:

- ConversationTurn
- IConversationMemory
- InMemoryConversationMemory
- ConversationQueryBuilder
- ConversationQuestionAnsweringService

The original /ask flow remains untouched.

Current Question alone is insufficient for follow-up questions.

Implemented strategy:

Previous Questions
+
Current Question

Example:

What is attention?

How does it relate to transformers?

How does it compare to RNNs?

The resulting retrieval query is embedded and sent to Qdrant.

Prompt contains:

Conversation History
+
Retrieved Context
+
Current Question

This allows the LLM to resolve references such as:

- it
- this
- that
- they

using prior conversation turns.

---

Successfully handled:

What is attention?

How does it relate to transformers?

without requiring the user to repeat:

attention

in subsequent questions.

---

Questions are used during retrieval.

Question + Answer pairs are used during prompt construction.

Answers are intentionally excluded from retrieval query construction to reduce embedding noise.

---

Questions are used during retrieval.

Question + Answer pairs are used during prompt construction.

Answers are intentionally excluded from retrieval query construction to reduce embedding noise.

---

Not implemented:

- Query Rewriting
- Memory Summarization
- Semantic Memory
- Persistent Conversation Storage

---

# Sprint 7 Retrospective

## What Went Well

- Streaming implementation was straightforward.
- Existing architecture adapted cleanly.
- Qdrant integration remained unaffected.
- Conversation memory provided immediate value.

## Challenges

- Streaming SSE testing.
- Ollama response deserialization.
- Deciding how history should influence retrieval.

## Key Learnings

- Streaming improves perceived latency.
- Memory-aware retrieval is more effective than memory-only prompting.
- Conversation memory and knowledge retrieval should remain independent concerns.
- Prompt growth becomes a new architectural consideration.

## Future Directions

- Evaluation Framework
- Query Rewriting
- Memory Summarization
- Semantic Memory
- Agentic Retrieval
---

## Related

- **ADRs**: [ADR-021 Conversational Memory](../adr/ADR-021-conversational-memory.md)
- **Concepts**: [Conversational Memory](../concepts/11-Conversational-memory.md)
- **Previous Sprint**: [Sprint 07A — Streaming](Sprint-07A.md)
- **Next Sprint**: [Sprint 08 — Evaluation](Sprint-08.md)
