# Vector Databases

## What Problem Do They Solve?

Embeddings are expensive to generate.

Without a vector database:

Document
↓
Chunk
↓
Embedding
↓
Memory

Application restart:
↓
Generate embeddings again

With a vector database:

Document
↓
Chunk
↓
Embedding
↓
Vector Database

Application restart:
↓
Vectors already available

## Key Concepts

- Collection
- Vector
- Metadata
- Similarity Search
- Approximate Nearest Neighbour (ANN)

## Qdrant Terminology

Table
↓
Collection

Row
↓
Point

Columns
↓
Payload

Indexes
↓
HNSW