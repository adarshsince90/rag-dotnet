# 04 - Embeddings And Vector Search

# Purpose of This Document

Embeddings and vector search are the foundation of modern RAG systems.

Without embeddings:

```text
No Semantic Search

No Similarity Search

No Meaning-Based Retrieval

No Effective RAG
```

This document explains embeddings and vector retrieval from first principles so that you can confidently answer both conceptual and technical assessment questions.

---

# Why This Topic Matters

Many developers know how to call:

```csharp
GenerateEmbeddingAsync(...)
```

but struggle to explain:

```text
What is an embedding?

Why do embeddings work?

How does vector search work?

How does Qdrant find relevant chunks?

What is cosine similarity?

Why not use SQL LIKE?
```

Understanding these concepts is critical because embeddings are the bridge between:

```text
Human Language
```

and

```text
Machine Search
```

---

# The Fundamental Problem

Computers are excellent at:

```text
Numbers

Mathematics

Comparisons
```

Computers are poor at:

```text
Meaning

Concepts

Language Understanding
```

---

# Example

Consider two sentences:

```text
How does self-attention work?
```

and

```text
Explain the attention mechanism in transformers.
```

Humans immediately recognize:

```text
Same topic
```

A traditional database does not.

---

# Why Keyword Search Is Not Enough

Traditional search relies on exact words.

Example:

Document:

```text
The Transformer uses a self-attention mechanism.
```

Question:

```text
How does attention work?
```

Keyword search:

```text
May Work
```

---

Question:

```text
How does focus mechanism work?
```

Keyword search:

```text
Fails
```

Because:

```text
focus ≠ attention
```

even though their meanings are related.

---

# The Core Idea

We need a way to represent:

```text
Meaning
```

rather than:

```text
Words
```

This is exactly what embeddings provide.

---

# What Is An Embedding?

An embedding is a numerical representation of meaning.

Example:

```text
Self-Attention
```

becomes:

```text
[0.14, -0.72, 0.56, ...]
```

The embedding vector captures semantic information.

---

# Simple Definition

> An embedding is a vector of numbers that represents the semantic meaning of text.

---

# Why Convert Text Into Numbers?

Computers cannot directly compare meaning.

However, computers can compare:

```text
Vectors

Coordinates

Distances

Angles
```

Embeddings convert:

```text
Language
```

into:

```text
Mathematics
```

---

# Mental Model

Imagine assigning coordinates.

Example:

```text
Dog
```

might map to:

```text
(10, 20)
```

while:

```text
Cat
```

might map to:

```text
(11, 19)
```

and:

```text
Airplane
```

might map to:

```text
(900, 750)
```

---

The system learns:

```text
Dog ≈ Cat

Dog ≠ Airplane
```

because similar concepts appear near each other.

---

# What Does An Embedding Really Represent?

An embedding does NOT represent:

```text
Characters

Keywords

Grammar Rules
```

It represents:

```text
Meaning

Concepts

Relationships
```

---

# Example

These phrases:

```text
Self-Attention

Attention Mechanism

Transformer Attention
```

produce vectors that are relatively close together.

---

These phrases:

```text
Relational Database

Cooking Recipe

Football Match
```

produce very different vectors.

---

# Vector Space Intuition

Embeddings live inside something called:

```text
Vector Space
```

Think of vector space as a giant map.

---

# Example

Animals:

```text
Dog
Cat
Tiger
Lion
```

might form one region.

---

Vehicles:

```text
Car
Bus
Truck
```

might form another region.

---

Technology:

```text
CPU
GPU
Transformer
Embedding
```

might form another cluster.

---

# Key Insight

In vector space:

```text
Similar Meaning
=
Closer Together
```

---

```text
Different Meaning
=
Further Apart
```

---

# How Does Vector Search Work?

When a user asks a question, the system does not search by keywords. It conducts a **geometric similarity search**:

```text
User Question
      ↓
Embedding Model (nomic-embed-text)
      ↓
Query Vector (768 Dimensions)
      ↓
Compare With Stored Chunk Vectors
      ↓
Calculate Similarity / Distance
      ↓
Sort By Proximity (Top-K)
```

---

# Distance Metrics in Vector Space

To determine how "close" two vectors are, vector engines use mathematical distance metrics:

### 1. Cosine Similarity (Default in RagDemo)

Measures the cosine of the angle between two vectors, regardless of their magnitude:

$$\text{Cosine Similarity} = \frac{\mathbf{u} \cdot \mathbf{v}}{\|\mathbf{u}\| \|\mathbf{v}\|}$$

- **Score Range**: `-1.0` (opposite) to `+1.0` (identical).
- **In Text Retrieval**: Normalized embeddings yield scores between `0.0` and `1.0`.
- **Advantage**: Length of text chunks does not distort semantic similarity.

### 2. Dot Product

$$\mathbf{u} \cdot \mathbf{v} = \sum_{i=1}^{n} u_i v_i$$

- If vectors are normalized to unit length ($\|\mathbf{u}\| = 1$), dot product equals cosine similarity.
- Extremely fast to compute.

### 3. Euclidean Distance (L2)

Measures straight-line geometric distance between points:

$$\text{Distance} = \sqrt{\sum_{i=1}^{n} (u_i - v_i)^2}$$

---

# How Qdrant Finds Relevant Chunks

Traditional databases use B-Trees for indexed lookup. However, high-dimensional vector spaces (e.g. 768 dimensions) suffer from the **curse of dimensionality**, making exact searches ($O(N)$ linear scans) prohibitively slow.

Qdrant uses **Approximate Nearest Neighbors (ANN)** via **HNSW** (Hierarchical Navigable Small World):

```text
Multi-Layer Graph Structure:

Top Layer:    [O] -----------------------> [O]         (Coarse skips)
                    \                    /
Mid Layer:    [O] ----> [O] ----------> [O] ----> [O]  (Medium skips)
                 \     /   \           /   \     /
Bottom Layer: [O]-[O]-[O]-[O]-[O]-[O]-[O]-[O]-[O]-[O] (Dense neighborhood)
```

### Why HNSW is Effective:
- **Sub-linear search time**: Performs retrieval in $O(\log N)$ time.
- **High Recall**: Finds 95%+ of true nearest neighbors in milliseconds.
- **Metadata Filtering**: Qdrant applies payload filters directly within HNSW graph traversal.

---

# Why Not Use SQL `LIKE` or Full-Text Search?

| Capability | SQL `LIKE` / Keywords | Vector Search (Dense) |
|---|---|---|
| **Exact Model Numbers** (e.g., `RTX-4090`) | ✅ Perfect | ⚠️ May struggle if out-of-vocabulary |
| **Synonyms & Paraphrasing** | ❌ Fails completely | ✅ Finds conceptual matches |
| **Cross-Lingual Matching** | ❌ None | ✅ Supported if multilingual model |
| **Typo Tolerance** | ❌ Strict failure | ✅ Robust to minor spelling variances |
| **Intent Understanding** | ❌ None | ✅ Captures query intent |

> **Best Practice (Enterprise Evolution)**: Combine both in **Hybrid Retrieval** (Dense Semantic + Sparse BM25/Full-text) using Reciprocal Rank Fusion (RRF).

---

# Project Implementation Details

In our .NET 10 RAG solution:

- **Embedding Model**: `nomic-embed-text` (via Ollama local AI).
- **Vector Dimension**: `768` floating-point numbers.
- **Storage Evolution**:
  - *Sprint 3*: `InMemoryChunkStore` (brute-force cosine similarity for rapid prototyping).
  - *Sprint 6*: `QdrantVectorStore` (persistent HNSW indexing via gRPC on port `6334`).
- **Retrieval Threshold**: Minimum similarity `0.55`.
- **Confidence Gap**: `0.036` to separate high-confidence results from ambient noise.
- **Top-K**: Default `5` chunks retrieved per question.

---

# Common Assessment Questions

## What is an embedding dimension?
> The number of numerical features generated per vector (e.g., 768 for nomic-embed-text, 1536 for text-embedding-3-small). Each dimension encodes subtle semantic relationships learned during model training.

---

## Why did you transition from InMemory storage to Qdrant?
> Generating embeddings for multiple PDFs took ~10 minutes on startup with InMemoryChunkStore. Qdrant persists vectors independently of the application lifecycle, enables instant restarts, scales to millions of vectors, and supports HNSW index search.

---

## Can you change the embedding model after indexing documents?
> **No.** All vectors in a collection must share the exact same vector space and dimensionality. If you switch embedding models (e.g., from `nomic-embed-text` to `all-minilm`), the entire collection must be re-indexed.
