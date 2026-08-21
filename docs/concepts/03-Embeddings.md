# Embeddings

## Definition

An embedding is a numerical representation of meaning.

Text:

"The headquarters are in Germany"

↓

Vector:

[0.12, -0.45, ...]

---

## Purpose

Transform language into mathematics.

Embeddings allow computers to compare meaning rather than exact words.

---

## Embedding Model

Input:

Text

Output:

Vector

Example:

nomic-embed-text

---

## Ingestion

Document
↓
Chunk
↓
Embedding Generation
↓
Stored Vector

---

## Retrieval

Question
↓
Question Embedding
↓
Similarity Comparison
↓
Relevant Chunks


## Important Observation

Embedding generation is usually the most expensive
part of document indexing.

Observed:

629 chunks
↓
~575 seconds

Embedding generation is an indexing concern,
not a retrieval concern.