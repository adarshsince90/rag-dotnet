# AI Engineering Learning Path

## Stage 1: Traditional Retrieval

### Concepts

- Chunking
- Keywords
- Ranking
- TF-IDF

### Status

✅ Complete

---

## Stage 2: Semantic Retrieval

### Concepts

- Embeddings
- Vectors
- Similarity Search
- Cosine Similarity

### Status

✅ Complete

---

## Stage 3: Hybrid Retrieval

### Concepts

- Keyword Search
- Semantic Search
- Ranking Fusion

### Status

✅ Complete

---

## Stage 4: LLM Integration

### Concepts

- Context Grounding
- Prompt Construction
- Hallucination Reduction
- Context Windows

### Status

✅ Complete

---

## Stage 5: Tool Calling

### Concepts

- Function Calling
- Tool Definitions
- Tool Selection
- Tool Execution

### Status

⬜ Planned

---

## Stage 6: Agents

### Concepts

- ReAct Pattern
- Planning
- Reasoning
- Action Loops

### Status

⬜ Planned

---

## Stage 7: Agent Workflows

### Concepts

- LangGraph Ideas
- State Machines
- Workflow Nodes
- Durable Execution

### Status

⬜ Planned

---

## Stage 8: Multi-Agent Systems

### Concepts

- Supervisor Agent
- Specialized Agents
- Agent Collaboration

### Status

⬜ Planned

---

## Stage 9: Evaluation & Observability

### Concepts

- Tracing
- Latency Monitoring
- Retrieval Diagnostics
- Prompt Evaluation
- LangSmith Concepts

### Status

⬜ Planned

### Observation

Retrieval quality matters more than model quality.

The LLM could only answer questions for which
relevant chunks were successfully retrieved.

Example:

Question:
nagarro based in

The retrieval step failed to include the
headquarters chunk.

The LLM therefore correctly responded:

"I could not find the answer..."

# Sprint 04 Learnings

## Retrieval Quality Drives Answer Quality

A powerful LLM cannot compensate for poor retrieval.

If relevant context is not retrieved,
correct answers become impossible.

---

## Semantic Search Is Not Perfect

Vector retrieval can struggle with:

- Rare entities
- Small chunks
- Weak contextual associations

---

## Chunking Is Critical

Retrieval quality depends on:

- Chunk size
- Chunk boundaries
- Content overlap

more than initially expected.

---

## Hallucination Prevention

Prompt engineering successfully prevented the model from answering questions unsupported by retrieved context.

Example:

Question:
Who is Google CEO?

Retrieved Chunks:
None

Answer:

I could not find the answer in the provided documents.

---

## Most Important Lesson

RAG consists of multiple cooperating systems:

- Chunking
- Embeddings
- Retrieval
- Prompt Engineering
- Generation

A weakness in any stage impacts overall answer quality.