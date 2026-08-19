# Retrieval Augmented Generation (RAG)

## Definition

RAG combines:

- Retrieval
- Generation

into a single workflow.

---

## Why RAG Exists

LLMs do not automatically know company documents.

They only know information included in the prompt.

RAG provides relevant context before answer generation.

---

## Flow

Question
↓
Retriever
↓
Top Chunks
↓
Prompt Construction
↓
LLM
↓
Answer

---

## Retrieval vs Generation

Retrieval:

Finds relevant information.

Generation:

Converts information into natural language.

---

## Example

Question:

Where is Nagarro based?

Retrieved Context:

The headquarters are in Germany.

Generated Answer:

Nagarro is headquartered in Germany.

---

## Benefits

- Reduced hallucinations
- Grounded answers
- Domain-specific knowledge
- No retraining required