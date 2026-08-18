# Sprint 03 - Embeddings & Semantic Retrieval

## Goal

Move from keyword-based retrieval to semantic retrieval using embeddings and vector similarity.

---

## Features Implemented

### Embedding Infrastructure

- IEmbeddingGenerator
- OllamaEmbeddingGenerator
- Ollama configuration support
- nomic-embed-text integration

### Embedding Generation

- GenerateEmbeddingsService
- Embedding generation endpoint
- Chunk embedding enrichment

### Storage

- IChunkStore
- InMemoryChunkStore

### Semantic Retrieval

- Cosine Similarity implementation
- VectorRetriever
- RetrievalResultProcessor
- Similarity threshold filtering
- Top-K processing

### Configuration

RetrievalOptions

- TopK
- MinimumSimilarity

---

## New Architecture

Document
↓
Chunk Provider
↓
DocumentChunk
↓
Embedding Generator
↓
Embedding Vector
↓
Chunk Store
↓
Vector Retriever
↓
Cosine Similarity
↓
Top-K Results

---

## Learning Objectives Achieved

### Embeddings

Text can be represented numerically as vectors.

Example:

Text:
"The headquarters are in Germany"

↓

Embedding:
[0.12, -0.45, ...]

---

### Semantic Search

Traditional Retrieval:

Word Matching

Semantic Retrieval:

Meaning Matching

---

### Cosine Similarity

Used to measure similarity between:

- Question embedding
- Chunk embedding

Range:

-1.0 to 1.0

Typical retrieval values:

0.4 - 0.9

---

## Experiments

### Successful

Query:

company based in?

Retrieved:

The headquarters are in Germany

Even though no exact word match existed.

---

### Interesting Observation

Query:

Where are Nagarro headquarters?

Ranked:

1. Nagarro was founded in 1996
2. Nagarro is a digital engineering company

instead of:

The headquarters are in Germany

---

## Lessons Learned

### Embeddings Are Not Magic

Semantic search improves retrieval but does not guarantee perfect ranking.

---

### Chunking Matters

Retrieval quality is highly dependent on chunk boundaries.

Bad:

Chunk A:
Nagarro founded in 1996

Chunk B:
Headquarters in Germany

Better:

Single larger chunk containing both facts.

---

### Entity Names Can Influence Ranking

Vector search may strongly favor chunks containing entity names.

Example:

"Nagarro"

appeared to dominate ranking decisions.

---

### Cosine Similarity Is A Ranking Signal

Cosine similarity should not be interpreted as confidence.

Example:

Unrelated query:

Who is Google CEO?

Still produced scores around:

0.4 - 0.5

Scores must be interpreted relatively.

---

### Retrieval Quality Depends On

- Embeddings
- Chunking
- TopK
- Similarity Threshold
- Query Formulation

## Key Insight

Retrieval quality depends not only on the embedding model but also on:

- Chunking strategy
- Entity handling
- Similarity thresholds
- Top-K selection
- Query formulation

The project demonstrated that semantic retrieval significantly improves search quality, but does not completely eliminate retrieval challenges.

This observation motivates future exploration of:

- Hybrid Retrieval
- Reranking
- Improved Chunking
- Vector Databases

---

## Future Improvements

- Hybrid Retrieval
- Chunk Overlap
- Reranking
- Vector Database

---

## Conclusion

Sprint 03 successfully transformed the system from keyword retrieval into semantic retrieval using vector embeddings and cosine similarity.

Semantic retrieval improves meaning-based matching but can sometimes underweight important entities. Combining keyword and vector retrieval may provide better overall retrieval quality by leveraging the strengths of both approaches.