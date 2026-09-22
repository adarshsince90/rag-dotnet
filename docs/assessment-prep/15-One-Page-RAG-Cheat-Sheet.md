# 15 - One-Page RAG Cheat Sheet

# Core Pipeline

```text
Question
 ↓
Embedding
 ↓
Vector Search
 ↓
Top-K Retrieval
 ↓
Prompt Construction
 ↓
LLM
 ↓
Answer
```

---

# Key Components

## Embedding

Converts text into vectors.

Purpose:

```text
Semantic Search
```

---

## Vector Database

Stores embeddings.

Example:

```text
Qdrant
```

---

## Chunking

Break large documents into smaller pieces.

Purpose:

```text
Better Retrieval Precision
```

---

## Retrieval

Find relevant chunks.

Purpose:

```text
Supply Context
```

---

## Prompt

Contains:

```text
System Prompt

Conversation History

Retrieved Context

Question
```

---

## Memory

Stores prior interactions.

Purpose:

```text
Conversation Continuity
```

---

## Streaming

Sends generated tokens incrementally.

Technology:

```text
SSE
```

---

# Evaluation

## Grounding

Answer supported by evidence.

---

## Hallucination

Unsupported information.

---

## Retrieval Quality

Measures:

```text
Precision

Recall

Coverage
```

---

# Advanced RAG

## Query Rewriting

Improve retrieval query.

---

## Query Expansion

Generate related searches.

---

## Multi-Query Retrieval

Multiple searches for one request.

---

## Hybrid Search

```text
Dense Search
+
Keyword Search
```

---

## Re-Ranking

Improve result ordering.

---

## Reflection

Model reviews its own answer.

---

## Agentic RAG

Dynamic retrieval and reasoning.

---

# Production Checklist

✅ Authentication

✅ Authorization

✅ Caching

✅ Monitoring

✅ Tracing

✅ Evaluation

✅ Backups

✅ Tenant Isolation

✅ Security

✅ Governance

---

# Most Important Lessons

```text
Retrieval Quality > Bigger Models

Prompt Engineering Matters

Evaluation Is Essential

Streaming Improves UX

Observability Drives Improvement

Architecture Is Trade-Offs
```
