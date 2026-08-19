# Prompt Engineering

## Purpose

Guide the LLM to use retrieved context rather than its own pre-trained knowledge.

---

## Prompt Structure

Instructions
↓
Context
↓
Question

---

## Example

You are a helpful assistant.

Answer ONLY using the provided context.

Context:
The headquarters are in Germany.

Question:
Where is the company based?

---

## Hallucination Prevention

Prompt includes:

If the answer cannot be found in the provided context,
reply with:

"I could not find the answer in the provided documents."

This encourages grounded responses.

---

## Learning

Prompt quality significantly influences answer quality.

Prompting is a critical component of RAG systems.
