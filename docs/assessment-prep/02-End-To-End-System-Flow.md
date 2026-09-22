# 02 - End-To-End System Flow

# Purpose of This Document

One of the most common assessment and interview questions is:

> "Walk me through your solution end-to-end."

Many developers understand individual components such as embeddings, vector databases, or prompt engineering, but struggle to explain how everything works together.

This document provides a complete mental model of the system from document ingestion to answer generation.

After studying this document, you should be able to confidently explain:

- What happens when a PDF is uploaded?
- What happens when a user asks a question?
- How is information retrieved?
- How is the final prompt constructed?
- How does conversational memory work?
- How does the answer reach the browser?

---

# Big Picture

Our solution is built around two major pipelines:

## Pipeline 1 - Knowledge Creation

This happens when documents are processed.

```text
PDF
 ↓
Text Extraction
 ↓
Chunking
 ↓
Embedding Generation
 ↓
Vector Database Storage
```

Goal:

```text
Convert documents into searchable knowledge.
```

---

## Pipeline 2 - Question Answering

This happens every time a user asks a question.

```text
Question
 ↓
Question Embedding
 ↓
Similarity Search
 ↓
Retrieve Relevant Chunks
 ↓
Prompt Construction
 ↓
LLM Generation
 ↓
Streaming Response
```

Goal:

```text
Generate grounded answers.
```

---

# Mental Model

Think of the system as a digital library.

## During Ingestion

The librarian reads every book and creates:

```text
Topic Cards
Keywords
Summaries
References
```

and stores them in a searchable catalog.

---

## During Question Answering

A user asks:

```text
Tell me about self-attention.
```

The librarian:

```text
Searches catalog
Finds relevant pages
Collects references
Hands them to expert
Expert answers
```

This is exactly how RAG works.

---

# Pipeline 1 - Knowledge Creation

---

# Step 1 - Document Upload

A PDF enters the system.

Example:

```text
attention-is-all-you-need.pdf
```

At this point the system only has:

```text
Binary PDF
```

The LLM cannot understand PDFs directly.

We must first extract text.

---

# Step 2 - Text Extraction

The ingestion pipeline extracts readable text.

```text
PDF
 ↓
Text Extraction
 ↓
Raw Text
```

Example:

```text
Attention Is All You Need introduces the Transformer model...
```

Now the content becomes machine-readable.

---

# Why We Cannot Stop Here

A complete research paper may contain:

```text
5,000
10,000
20,000+
words
```

Embedding the entire document as a single vector is ineffective.

Why?

Because one vector attempts to represent:

```text
Introduction
Architecture
Training
Evaluation
Results
Conclusion
```

all together.

The semantic signal becomes diluted.

---

# Step 3 - Chunking

The document is split into smaller sections.

Example:

```text
Chunk 1
Introduction

Chunk 2
Transformer Architecture

Chunk 3
Self Attention

Chunk 4
Training Strategy

Chunk 5
Evaluation Results
```

---

# Why Chunking Exists

Without chunking:

```text
Entire Document
    ↓
Single Embedding
```

Retrieval quality becomes poor.

---

With chunking:

```text
Document
    ↓
Hundreds Of Chunks
    ↓
Hundreds Of Embeddings
```

We can retrieve only the sections that matter.

---

# Chunking Analogy

Imagine a textbook.

Question:

```text
What is self-attention?
```

Would you retrieve:

```text
Entire textbook?
```

or

```text
Chapter on self-attention?
```

Chunking allows retrieval of the relevant chapter.

---

# Step 4 - Embedding Generation

Each chunk becomes an embedding.

Example:

```text
Self-attention allows a model to...
```

becomes:

```text
[0.23, -0.17, 0.72, ...]
```

---

# What Is An Embedding?

An embedding is a numerical representation of meaning.

The vector captures semantic relationships.

Example:

```text
Self-attention

Attention Mechanism

Transformer Attention
```

These phrases produce vectors that are close together.

---

# What Does The Embedding Model Do?

Input:

```text
Text
```

Output:

```text
Vector
```

Example:

```text
"This is attention."
         ↓
Embedding Model
         ↓
1536-dimensional vector
```

The exact dimensionality depends on the embedding model.

---

# Step 5 - Store In Qdrant

The chunk and embedding are stored.

Example:

```json
{
  "chunkId": "123",
  "document": "attention.pdf",
  "text": "Self-attention allows...",
  "vector": [...]
}
```

Stored in:

```text
Qdrant
```

---

# Why Use Qdrant?

Qdrant specializes in:

```text
Vector Storage

Similarity Search

Nearest Neighbor Search
```

Traditional databases excel at:

```text
Exact Match Queries
```

Vector databases excel at:

```text
Meaning-Based Queries
```

---

# Knowledge Creation Pipeline Summary

```text
PDF
 ↓
Extract Text
 ↓
Chunk Document
 ↓
Generate Embeddings
 ↓
Store In Qdrant
```

At this point the system has searchable knowledge.

---

# Pipeline 2 - Question Answering

---

# Step 1 - User Asks Question

Example:

```text
What is self-attention?
```

This question enters the API.

---

# Important Principle

The system does NOT immediately call the LLM.

First it tries to find relevant knowledge.

---

# Step 2 - Generate Question Embedding

The same embedding model is used.

Question:

```text
What is self-attention?
```

becomes:

```text
[0.54, -0.83, 0.91, ...]
```

---

# Why Use The Same Embedding Model?

Because:

```text
Document Embeddings
Question Embeddings
```

must exist in the same vector space.

Otherwise similarity calculations become meaningless.

---

# Step 3 - Similarity Search

The question embedding is sent to Qdrant.

```text
Question Vector
       ↓
Qdrant Search
       ↓
Nearest Vectors
```

---

# What Happens Internally?

Qdrant computes similarity scores.

Example:

```text
Chunk A → 0.92

Chunk B → 0.87

Chunk C → 0.84

Chunk D → 0.41
```

The highest-scoring chunks are returned.

---

# Step 4 - Top K Retrieval

Example:

```text
Top 5 Chunks
```

might return:

```text
Chunk 32
Self Attention Definition

Chunk 54
Attention Mathematics

Chunk 71
Transformer Architecture

Chunk 88
Attention Layers

Chunk 110
Model Training
```

These chunks become context.

---

# Why Top K Matters

Too few chunks:

```text
Missing Information
```

Too many chunks:

```text
Prompt Bloat
Higher Cost
More Noise
```

Retrieval is a balance.

---

# Step 5 - Conversation Memory

Our system supports multi-turn conversations.

Example:

```text
Q1:
What is self-attention?

Q2:
What are its advantages?
```

The second question alone is ambiguous.

```text
Its?
```

What does "its" refer to?

---

The conversation store remembers:

```text
Previous Questions

Previous Answers
```

and injects them into prompt construction.

---

# Step 6 - Prompt Construction

This is one of the most important steps.

The LLM receives much more than the current question.

---

# Prompt Structure

```text
System Instructions
+
Conversation History
+
Retrieved Chunks
+
Current Question
```

---

# Example Prompt

```text
You are a helpful assistant.

Use only the provided context.

Conversation History:
...

Context:
Chunk 1...
Chunk 2...
Chunk 3...

Question:
What is self-attention?
```

---

# Why Prompt Construction Matters

Poor prompts lead to:

```text
Hallucinations

Ignored Context

Poor Answers
```

Retrieval quality and prompt quality are equally important.

---

# Step 7 - Send To LLM

The final prompt is sent to:

```text
Ollama
```

which hosts the language model.

---

# The LLM's Job

The LLM does NOT perform retrieval.

Retrieval already happened.

The LLM's job is:

```text
Read Context

Understand Question

Generate Response
```

---

# Step 8 - Token Generation

The response is generated incrementally.

Example:

```text
Self
```

then

```text
Self-attention
```

then

```text
Self-attention is
```

and so on.

The answer exists as a stream of tokens.

---

# Streaming Pipeline

---

# Why Stream?

Without streaming:

```text
Wait 30 Seconds
 ↓
Entire Answer Appears
```

Poor user experience.

---

With streaming:

```text
Self
Self-attention
Self-attention is
...
```

Users immediately see progress.

---

# Server-Sent Events (SSE)

Our API uses:

```text
Content-Type: text/event-stream
```

---

Events:

```text
event: retrieval
data: {...}

event: token
data: "Self"

event: token
data: "attention"

event: completed
data: {...}
```

---

# Browser Rendering

The browser:

```text
Receives Event
       ↓
Parses Event
       ↓
Updates UI
```

until completion.

---

# Complete Sequence Diagram

```text
User
 │
 │ Ask Question
 ▼

API
 │
 │ Generate Embedding
 ▼

Embedding Model
 │
 ▼

Qdrant
 │
 │ Similarity Search
 ▼

Relevant Chunks
 │
 ▼

Prompt Builder
 │
 │ Add:
 │ - System Prompt
 │ - Memory
 │ - Context
 │ - Question
 ▼

Ollama
 │
 │ Generate Tokens
 ▼

SSE Endpoint
 │
 ▼

Browser UI
 │
 ▼

Streaming Answer
```

---

# Failure Scenarios

Understanding failure modes demonstrates deeper knowledge.

---

## Poor Chunking

Problem:

```text
Information split incorrectly
```

Result:

```text
Poor Retrieval
```

---

## Poor Embeddings

Problem:

```text
Semantic meaning captured badly
```

Result:

```text
Wrong chunks retrieved
```

---

## Poor Retrieval

Problem:

```text
Irrelevant context
```

Result:

```text
Poor answers
```

---

## Excessive Context

Problem:

```text
Too many chunks
```

Result:

```text
Prompt dilution
Token waste
Higher latency
```

---

## Missing Conversation Memory

Problem:

```text
Follow-up questions lose context
```

Result:

```text
Confusing responses
```

---

# End-To-End Flow In One Diagram

```text
PDF
 ↓
Extract Text
 ↓
Chunk
 ↓
Generate Embeddings
 ↓
Store In Qdrant

--------------------------------

Question
 ↓
Generate Question Embedding
 ↓
Vector Search
 ↓
Top K Chunks
 ↓
Conversation Memory
 ↓
Prompt Construction
 ↓
Ollama
 ↓
Token Stream
 ↓
SSE Endpoint
 ↓
Browser UI
```

---

# Assessment Questions

## Q: What happens when a PDF is uploaded?

**Answer:**

The PDF is parsed into text, split into chunks, converted into embeddings, and stored in Qdrant for future semantic retrieval.

---

## Q: What happens when a user asks a question?

**Answer:**

The question is embedded, Qdrant performs similarity search, the top matching chunks are retrieved, conversation history is added, a prompt is constructed, and the LLM generates a grounded answer that is streamed back to the UI.

---

## Q: Why not send the question directly to the LLM?

**Answer:**

The LLM may lack knowledge of the uploaded documents and may hallucinate. Retrieval provides relevant context before generation.

---

## Q: Why generate embeddings for both documents and questions?

**Answer:**

Embedding both into the same vector space allows semantic similarity search between questions and document content.

---

## Q: Where does conversation memory fit?

**Answer:**

Conversation history is injected during prompt construction, allowing the system to understand follow-up questions and maintain context across multiple turns.

---

# 60-Second Assessment Answer

> "Our solution consists of two pipelines. During ingestion, PDFs are converted into text, chunked, embedded, and stored in Qdrant. During question answering, the user's question is embedded and used for vector similarity search against Qdrant. The top matching chunks are retrieved and combined with conversation history and system instructions to construct a prompt. The prompt is sent to the LLM running in Ollama, which generates a grounded response. Tokens are streamed through an SSE endpoint and rendered incrementally in the browser UI. The overall architecture improves answer quality by grounding responses in retrieved document context rather than relying solely on the model's training knowledge."