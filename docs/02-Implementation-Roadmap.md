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

# Sprint 04 Learnings

## Retrieval Is More Important Than Expected

Most answer quality issues were retrieval issues.

## Chunking Is Critical

Changing chunking produced larger improvements than changing models.

## Not All Questions Are Equal

Definition Questions

What is BERT?

↓

Small Context

Summary Questions

Summarize Transformer Paper

↓

Large Context

This suggests future need for dynamic retrieval strategies.

## Semantic Search Tolerates Minor Errors

Examples:

✅ BERTs
✅ BERTing
✅ transfomers

❌ BERTH

## Generation Is The Current Bottleneck

Retrieval:
~100-200ms

Generation:
~20-40s

Future streaming responses may improve user experience.

---

# Sprint 7A Learnings

## Learning 1

Perceived latency is often more important
than generation latency.

A 30-second response feels significantly faster
when users receive output immediately.

---

## Learning 2

Streaming and generation speed are different concepts.

Streaming:

Improves responsiveness.

Streaming does not:

Reduce total generation time.

---

## Learning 3

AI applications benefit greatly from token streaming.

This pattern is used by:

- ChatGPT
- Copilot
- Claude
- Gemini
- Perplexity

---

## Learning 4

SSE is an effective transport for LLM streaming.

Advantages:

- Lightweight
- Simple implementation
- Native browser support

---

## Learning 5

Modern RAG systems consist of three distinct pipelines.

Indexing Pipeline

Document
↓
Chunk
↓
Embedding
↓
Qdrant

Retrieval Pipeline

Question
↓
Search
↓
Context

Generation Pipeline

Prompt
↓
LLM
↓
Streaming Response

# Sprint 7A Retrospective

## What Went Well

- Ollama native streaming support worked well.
- SSE implementation was straightforward.
- Existing architecture required minimal changes.
- Retrieval layer remained unchanged.

## Challenges

- DTO deserialization mismatch.
- SSE client tooling limitations.
- Streaming endpoint testing.

## Key Learnings

- Streaming improves responsiveness significantly.
- Streaming does not reduce generation duration.
- SSE is sufficient for current requirements.
- Modern AI systems stream by default.

## Next Sprint

Sprint 7B

Conversation Memory

Goals:

- ConversationTurn model
- IConversationMemory
- Last 4 interactions
- Context-aware follow-up questions
- Memory-aware prompt builder

---

# Sprint 7B Learnings

## Learning 1

Conversation Memory and Knowledge Memory are different concepts.

Conversation Memory:

Stores prior interactions.

Knowledge Memory:

Stores retrieved document knowledge.

---

## Learning 2

History-aware retrieval can significantly improve retrieval quality.

Using:

Previous Questions
+
Current Question

provides stronger retrieval signals than current question alone.

---

## Learning 3

Prompt growth becomes a new system concern.

Memory introduces additional context.

Prompt size increases with conversation length.

---

## Learning 4

Query rewriting is not required initially.

Useful conversational behavior can be achieved through:

History-Aware Retrieval
+
History-Aware Prompting

without introducing an additional LLM call.

## Sprint 7A ✅

Streaming Responses

Completed:

- Ollama Streaming
- SSE Endpoint
- IAsyncEnumerable Support
- /ask/stream

---

## Sprint 7B ✅

Conversational Memory

Completed:

- ConversationTurn
- IConversationMemory
- InMemoryConversationMemory
- ConversationQueryBuilder
- History-Aware Retrieval
- Memory-Aware Prompting
- /ask/conversation/stream

---

## Sprint 7

A significant milestone was reached with the introduction of conversational memory.

The system now combines:

Knowledge Retrieval
+
Conversation Memory

Knowledge is retrieved from Qdrant while conversational context is maintained independently.

Important realization:

Memory and retrieval solve different problems.

Memory answers:

"What are we talking about?"

Retrieval answers:

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

Observed Limitation

History-aware retrieval can introduce topic bias.

Example:

Conversation:

- What is attention?
- How does it relate to transformers?

Followed by:

- What is human attention?

The retrieval system may continue retrieving
machine-learning-related attention content due
to strong conversational context.

Future enhancements may require:

- Topic Shift Detection
- Query Classification
- Memory Relevance Scoring
- Semantic Memory Retrieval

---

Evaluation Framework ✅
Memory Evaluation ✅
Grounding Evaluation ✅
Automated Benchmarking ✅
Dataset (12 Questions) ✅
Results Documentation ✅
PDF Extraction Improvement ✅