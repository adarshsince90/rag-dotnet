# Sprint 6
# Persistent Vector Storage (Qdrant)

## Goal

Replace the transient in-memory vector storage mechanism with a persistent vector database while preserving the existing retrieval and RAG architecture.

---

## Problem Statement

The existing implementation stored embeddings in an InMemoryChunkStore.

Startup flow:

Documents
↓
Chunking
↓
Embedding Generation
↓
InMemoryChunkStore

This approach introduced several limitations:

- Embeddings were lost on application restart.
- Startup required regenerating embeddings.
- Indexing five research papers required approximately 9-10 minutes.
- Storage was not scalable beyond application memory.

---

## Solution

Introduce Qdrant as the persistent vector database.

New architecture:

PDF Documents
↓
Chunking
↓
Embeddings
↓
Qdrant Vector Store
↓
Vector Retrieval
↓
Prompt Building
↓
LLM Response

---

## Implemented Components

### Domain

Added:

- IVectorStore

### Infrastructure

Added:

- QdrantOptions
- QdrantVectorStore

Features:

- Collection initialization
- Vector upsert
- Vector search
- Metadata storage

### Docker

Added:

- Qdrant Container
- Persistent storage volume

### Retrieval

Updated:

- VectorRetriever

Old:

Question
↓
Embedding
↓
InMemoryChunkStore
↓
Cosine Similarity

New:

Question
↓
Embedding
↓
Qdrant Search
↓
Retrieved Chunks

---

## Qdrant Collection Design

Collection:

ragdemo-documents

Vector Configuration:

- Dimension: 768
- Distance Metric: Cosine Similarity

Stored Payload:

{
  source,
  chunkIndex,
  content
}

Stored Separately:

- Embedding Vector

---

## Configuration

Qdrant:

{
  "Qdrant": {
    "BaseUrl": "http://localhost:6334",
    "CollectionName": "ragdemo-documents",
    "VectorSize": 768
  }
}

Retrieval:

{
  "Retrieval": {
    "TopK": 3,
    "MinimumSimilarity": 0.50,
    "ConfidenceGap": 0.036
  }
}

---

## Validation

### Corpus

Research Papers:

- Attention Is All You Need
- BERT
- Language Models Are Few-Shot Learners
- Additional Transformer Papers

Generated:

- 629 chunks

### Retrieval Validation

Comparison was performed between:

- InMemory Retrieval
- Qdrant Retrieval

Results:

- Same TopK matches
- Same ranking
- Nearly identical similarity scores

Observation:

Qdrant retrieval quality matched the previous in-memory implementation.

---

## Performance Findings

### Indexing

Embedding Generation:

~575843 ms
(~9.6 minutes)

Observation:

Embedding generation remains the dominant indexing cost.

### Retrieval

Observed Retrieval Time:

~70-250 ms

Observation:

Retrieval is no longer a significant bottleneck.

### Generation

Observed Generation Time:

~20-40 seconds

Current primary bottleneck:

LLM response generation

not vector retrieval.

---

## Architectural Learning

Sprint 6 introduced an important distinction:

### Indexing Pipeline

Documents
↓
Chunking
↓
Embeddings
↓
Qdrant

### Query Pipeline

Question
↓
Question Embedding
↓
Qdrant Search
↓
Prompt
↓
LLM

These pipelines are now independent.

---

## Known Limitations

### PDF Extraction

Extracted text occasionally loses whitespace.

Example:

attention mechanisms

becomes

attentionmechanisms

Retrieval quality remains acceptable but answer quality may improve with future text normalization.

### Retrieval Diagnostics

TotalChunks is currently unavailable after migration to Qdrant retrieval.

Future enhancement:

Retrieve collection statistics from Qdrant.

---

## Sprint Outcome

✅ Persistent vector storage

✅ Qdrant integration

✅ Collection initialization

✅ Metadata persistence

✅ Retrieval migration

✅ Startup optimization

✅ Retrieval validation

✅ Production-style vector architecture

Sprint Status:

COMPLETE


# Sprint 6 Retrospective

## What Went Well

- Qdrant integration succeeded.
- Retrieval quality remained unchanged.
- Persistent vectors eliminated startup indexing.
- Existing retrieval abstractions adapted well.
- Clean Architecture required minimal changes.

## Challenges

- gRPC vs REST endpoint confusion.
- Qdrant payload construction issues.
- Collection initialization workflow.
- PDF extraction quality.

## Key Learnings

- Vector databases improve persistence more than answer quality.
- Indexing and querying are separate pipelines.
- ANN retrieval produces comparable results for current corpus size.
- Embedding generation remains the most expensive indexing operation.

## Next Sprint

Sprint 7

Conversation Memory

Objectives:

- ConversationTurn model
- IConversationMemory
- Last 4 interactions
- Memory-aware prompt builder
- Conversational follow-up questions
---

## Related

- **ADRs**: [ADR-020 Qdrant Vector Storage](../adr/ADR-020-qdrant-vector-storage.md)
- **Concepts**: [Vector Database](../concepts/09-vector-database.md)
- **Previous Sprint**: [Sprint 05B � PDF Support](Sprint-05B.md)
- **Next Sprint**: [Sprint 07A � Streaming](Sprint-07A.md)
