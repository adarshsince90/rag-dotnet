# Sprint 04 - Retrieval Augmented Generation (RAG)

## Goal

Transform the application from a retrieval system into a complete Retrieval Augmented Generation (RAG) solution.

Previous state:

Question
↓
Retriever
↓
Chunks

Current state:

Question
↓
Retriever
↓
Relevant Chunks
↓
Prompt Builder
↓
LLM
↓
Generated Answer

---

## Features Implemented

### Chat Completion

- IChatCompletionService
- OllamaChatCompletionService
- gemma2:2b integration

### Prompt Engineering

- IPromptBuilder
- RagPromptBuilder

### RAG Orchestration

- QuestionAnsweringService

### Diagnostics

- Retrieval quality diagnostics
- Similarity threshold support
- Top-K retrieval

---

## Final Architecture

Question
↓
VectorRetriever
↓
Top-K Chunks
↓
Prompt Builder
↓
Gemma 2
↓
Answer

---

## Example

Question:

Where is the company based?

Retrieved Context:

- The headquarters are in Germany
- Nagarro is a digital engineering company

Generated Answer:

Nagarro is a digital engineering company based in Germany.

---

## Key Learnings

### Retrieval And Generation Are Different Problems

Retriever:

Responsible for finding relevant context.

LLM:

Responsible for generating natural language answers.

---

### Good Retrieval Enables Good Answers

The LLM can only answer based on the context it receives.

If retrieval misses the correct chunk, answer quality suffers.

---

### Hallucination Prevention Works

Prompt:

If answer cannot be found, reply:

"I could not find the answer in the provided documents."

Result:

Unrelated questions correctly produce:

"I could not find the answer in the provided documents."

instead of hallucinated answers.

---

## Interesting Observations

### Query

Where is Nagarro based?

Result:

Retrieval preferred chunks containing the word:

Nagarro

and missed:

The headquarters are in Germany

---

### Learning

The issue was not generation quality.

The issue was retrieval quality.

Specifically:

- Chunk design
- Entity handling
- Similarity ranking

---

## Sprint Outcome

A complete locally running RAG system was implemented using:

- Ollama
- nomic-embed-text
- Gemma 2
- Vector Search
- Prompt Based Grounding

without external cloud services.

### Key Insight

The project demonstrated that answer generation is relatively straightforward once high-quality retrieval is available.

Most answer-quality issues encountered during development were ultimately retrieval issues rather than LLM issues.

This reinforced the principle that successful RAG systems depend heavily on retrieval quality, chunking strategy, and contextual grounding.

----------
# Sprint 04 - Retrieval Augmented Generation (RAG)

## Goal

Transform the application from semantic retrieval into a complete RAG system capable of generating grounded answers using retrieved context.

## Implemented

### Chat Completion

- IChatCompletionService
- OllamaChatCompletionService
- Gemma 2 integration

### Prompt Engineering

- IPromptBuilder
- RagPromptBuilder

### Application Layer

- QuestionAnsweringService

### Startup Initialization

- Embeddings generated at startup
- InMemoryChunkStore populated automatically

### Diagnostics

- RetrievalMs
- GenerationMs
- TotalMs
- LLM Invoked

## Architecture

Question
↓
Retriever
↓
Relevant Chunks
↓
Prompt Builder
↓
LLM
↓
Generated Answer

## Chunking Laboratory

### Initial Problem

Line-based chunking produced poor retrieval results.

Example:

Where is Nagarro based?

Failed to retrieve:

The headquarters are in Germany

### Solution

Larger contextual chunks improved retrieval quality dramatically.

### Character Chunking Configuration

ChunkSize: 1000

ChunkOverlap: 200

### Findings

- Retrieval quality improved
- Larger chunks preserved context
- Overlap reduced boundary issues
- Retrieval became more stable

### BERT / Transformer Dataset Validation

Successfully answered:

- What is BERT?
- What is Attention?
- Explain Attention and Transformer
- What does BERT stand for?

### Misspelling Tolerance

Successful:

- BERTs
- BERTing
- transfomers

Failed:

- BERTH

Observation:

Semantic retrieval tolerates minor variations but not significant token changes.

### Summary Queries

Question:

Summarize attention-is-all-you-need.txt

Observation:

Summary queries require a different retrieval strategy than fact-based queries.

### Key Learnings

Retrieval quality has greater impact on answer quality than model quality.

Chunking strategy significantly affects retrieval effectiveness.

Not all questions require the same retrieval strategy.

Future concepts discovered:

- Dynamic TopK
- Query Classification
- Hybrid Retrieval
- Agent Routing