# ADR-013: Grounded Prompting (LLMs Must Use Retrieved Context)

**Status:** Accepted
**Sprint:** 4

## Context

LLMs can hallucinate answers that sound plausible but are not supported by the project documents. This is unacceptable for a RAG system.

## Decision

Prompts instruct the model to answer **only** from retrieved context.

If the answer cannot be found in the context, the model must reply with:

> "I could not find the answer in the provided documents."

## Rationale

- Reduces hallucination risk
- Keeps answers grounded in project documents
- Users can trust that answers come from their document corpus

## Consequences

- Model will refuse to answer questions outside the document corpus
- Answer quality depends heavily on retrieval quality
- Some valid general questions will be refused (by design)

## Related

- [Sprint 04](../sprint/Sprint-04.md) — LLM integration
- [ADR-012](ADR-012-prompt-separation.md) — Prompt construction separation
- [Prompting concept](../concepts/07-Prompting.md)
- [RAG concept](../concepts/06-RAG.md)
