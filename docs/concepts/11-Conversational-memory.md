# Conversational Memory

## Definition

Conversational memory allows an AI system to remember previous interactions during a conversation.

---

## Types of Memory

### Short-Term Memory

Stores recent interactions.

Example:

Last 4 turns.

Implemented in Sprint 7B.

---

### Summarized Memory

Stores compressed summaries of older interactions.

Not yet implemented.

---

### Semantic Memory

Stores memory embeddings in a vector database.

Not yet implemented.

---

## Current Implementation

ConversationId
↓
InMemoryConversationMemory
↓
Last 4 turns
↓
Prompt Injection

---

## Benefits

- Follow-up questions
- Reduced repetition
- More natural conversations