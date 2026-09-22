# 01 - RAG Executive Summary

## Purpose of This Document

This document provides a concise overview of Retrieval-Augmented Generation (RAG), the problem it solves, the major components involved, and how our implemented solution works end-to-end.

It serves as a **5-minute revision guide** before an assessment, interview, architecture review, or technical discussion.

---

# What Problem Are We Solving?

Large Language Models (LLMs) are powerful, but they have important limitations.

## 1. Knowledge Cutoff

An LLM only knows what it learned during training.

Example:

```text
Question:
What are the findings in my PDF uploaded today?

LLM:
I don't know.
```

The uploaded PDF was never part of the model's training dataset.

---

## 2. Hallucination Problem

When an LLM does not know the answer, it may still generate a confident response.

Example:

```text
Question:
What does section 8 of my company policy say?

LLM:
Section 8 discusses employee benefits...
```

Even if that information does not exist.

This behavior is called:

```text
Hallucination
```

---

## 3. Private Knowledge Problem

Organizations often need answers from:

- Internal documents
- Company policies
- Research papers
- Product documentation
- Knowledge bases

These documents are not available during model training.

---

# What Is RAG?

RAG stands for:

```text
Retrieval-Augmented Generation
```

Instead of asking the LLM to answer purely from memory:

```text
Question
    ↓
LLM
    ↓
Answer
```

RAG first retrieves relevant information:

```text
Question
    ↓
Retrieve Relevant Context
    ↓
LLM
    ↓
Grounded Answer
```

The answer is generated using both:

- Model knowledge
- Retrieved document context

---

# Simple Definition

> Retrieval-Augmented Generation (RAG) is a technique that combines information retrieval with language generation so that answers are based on external knowledge sources rather than only the model's training data.

---

# High-Level Architecture

```text
Documents
    ↓
Chunking
    ↓
Embeddings
    ↓
Vector Database

----------------------------

Question
    ↓
Question Embedding
    ↓
Vector Search
    ↓
Relevant Chunks Retrieved
    ↓
Prompt Construction
    ↓
LLM Generation
    ↓
Answer
```

---

# Core Components Of Our Solution

## 1. Document Ingestion

### Purpose

Convert documents into searchable knowledge.

### Input

```text
PDF Documents
```

### Output

```text
Chunks stored in Vector Database
```

---

## 2. Chunking

### Purpose

Split large documents into smaller pieces.

Example:

```text
100 Page PDF
    ↓
500 Chunks
```

### Why Chunking?

Embedding an entire document usually produces poor retrieval results because:

- Too much information is compressed into one vector
- Retrieval becomes less precise
- Context becomes too broad

Smaller chunks improve retrieval accuracy.

---

## 3. Embeddings

### Purpose

Convert text into numerical vectors.

Example:

```text
"Self-attention"

↓

[0.23, -0.51, 0.91, ...]
```

### Why?

Computers cannot compare meaning directly.

Embeddings allow semantic comparison.

Example:

```text
Self-attention

Attention Mechanism

Transformer Attention
```

These may use different words but have similar meanings.

Their vectors will be close together in vector space.

---

## 4. Vector Database

Our implementation uses:

```text
Qdrant
```

### Purpose

Store embeddings and perform similarity search.

Traditional search asks:

```text
Does this document contain the exact keyword?
```

Vector search asks:

```text
Which document is semantically similar?
```

This enables meaning-based retrieval.

---

## 5. Retrieval

When a user asks:

```text
What is self-attention?
```

The system:

1. Creates an embedding for the question
2. Searches Qdrant
3. Retrieves the most relevant chunks

Example:

```text
Question Vector
        ↓
Similarity Search
        ↓
Top 5 Chunks
```

These chunks become context for generation.

---

## 6. Prompt Construction

The LLM does not receive only the question.

The final prompt consists of:

```text
System Instructions
+
Retrieved Context
+
Conversation History
+
Current Question
```

Example:

```text
You are a helpful assistant.

Use only the provided context.

Context:
...

Question:
What is self-attention?
```

This helps ground the answer in actual retrieved content.

---

## 7. Generation

The LLM generates an answer using the retrieved context.

Our implementation uses:

```text
Ollama
```

for local model inference.

The answer generation process becomes:

```text
Question
    ↓
Retrieved Context
    ↓
Prompt
    ↓
LLM
    ↓
Answer
```

---

## 8. Streaming

Instead of waiting for the full answer:

```text
Wait 30 Seconds
        ↓
Entire Answer
```

we stream tokens as soon as they are generated.

```text
Self
Self-attention
Self-attention is
Self-attention is an
...
```

Our implementation uses:

```text
Server-Sent Events (SSE)
```

Benefits:

- Faster perceived performance
- Better user experience
- ChatGPT-like interaction

---

## 9. Conversation Memory

Questions are not treated as completely independent.

Example:

```text
Q1:
What is self-attention?

Q2:
What are its advantages?
```

The system understands that:

```text
its = self-attention
```

because conversation history is included in prompt construction.

### Benefits

- Multi-turn conversations
- Context awareness
- Follow-up questions
- Better user experience

---

## 10. Evaluation

Generating answers is not enough.

We must measure quality.

Our solution evaluates:

```text
Retrieval Quality

Grounding

Answer Quality

Execution Metrics

Response Metrics
```

This helps determine:

- Is retrieval working?
- Is the answer grounded?
- Is the response trustworthy?
- Is performance acceptable?

---

# End-To-End Flow

```text
PDF Document
     ↓
Document Parsing
     ↓
Chunking
     ↓
Embedding Generation
     ↓
Store In Qdrant

---------------------------------

User Question
     ↓
Generate Question Embedding
     ↓
Qdrant Similarity Search
     ↓
Retrieve Top K Chunks
     ↓
Construct Prompt
     ↓
Include Conversation History
     ↓
LLM Generation
     ↓
Stream Response Via SSE
     ↓
Browser UI
```

---

# Why RAG Works

Without RAG:

```text
Question
    ↓
LLM Memory
    ↓
Answer
```

Problems:

```text
Knowledge Cutoff

Hallucinations

No Access To Private Data
```

---

With RAG:

```text
Question
    ↓
Knowledge Retrieval
    ↓
Prompt Construction
    ↓
LLM
    ↓
Grounded Answer
```

Benefits:

```text
Access To Private Documents

Reduced Hallucinations

More Accurate Answers

Source Traceability

Up-to-Date Information

Better Trustworthiness
```

---

# Technologies Used In Our Implementation

## Backend

```text
ASP.NET Core
.NET
Clean Architecture
```

## AI Components

```text
Ollama
Embeddings
Prompt Construction
```

## Retrieval Layer

```text
Qdrant
Vector Search
```

## Communication

```text
SSE (Server-Sent Events)
```

## User Interface

```text
Static Browser UI
HTML
CSS
JavaScript
```

---

# What We Built

Our solution includes:

```text
✅ Clean Architecture

✅ PDF Ingestion

✅ Chunking

✅ Embeddings

✅ Qdrant Integration

✅ Semantic Retrieval

✅ Prompt Construction

✅ Conversational Memory

✅ SSE Streaming

✅ Evaluation Framework

✅ Retrieval Diagnostics

✅ Browser Chat UI

✅ Source Attribution
```

---

# Key Assessment Takeaways

If asked:

## What is RAG?

> RAG combines information retrieval with language generation to produce answers grounded in external knowledge sources.

---

## Why is RAG needed?

> LLMs have knowledge cutoffs, cannot access private documents, and may hallucinate. RAG retrieves relevant information and provides it to the model before answer generation.

---

## How does RAG reduce hallucinations?

> The model is instructed to generate answers using retrieved document context rather than relying solely on its internal knowledge.

---

## Why use a Vector Database?

> A vector database enables semantic similarity search, allowing the system to retrieve information based on meaning rather than exact keywords.

---

## Explain the flow in one sentence.

> Documents are chunked, embedded, and stored in Qdrant; user questions are embedded and used to retrieve relevant chunks, which are combined with conversation history and passed to the LLM to generate grounded responses.

---

# 30-Second Assessment Answer

> "Our system ingests PDF documents, splits them into chunks, generates embeddings, and stores them in Qdrant. When a user asks a question, the question is embedded and used for semantic retrieval of the most relevant chunks. Those chunks, together with conversation history and system instructions, are used to construct a prompt for the LLM running through Ollama. The generated answer is streamed to the UI using SSE. The solution also includes conversational memory, source attribution, retrieval diagnostics, evaluation capabilities, and follows Clean Architecture principles."