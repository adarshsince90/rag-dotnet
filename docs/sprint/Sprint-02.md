# Sprint 02 - Retrieval Ranking & Diagnostics

## Goal

Improve retrieval quality, visibility, and diagnostic capabilities before introducing semantic search and embeddings.

## Features Implemented

- RetrievalResult model
- RetrievalResponse model
- RetrievalDiagnostics model
- Top-K retrieval
- Ranking support
- Chunk metadata
  - Source
  - ChunkIndex
- Retrieval diagnostics
- Nullability cleanup
- Retrieval threshold handling
- Improved API response structure

## Architecture Changes

### New Domain Models

- RetrievalResult
- RetrievalResponse
- RetrievalDiagnostics

### Updated Models

DocumentChunk

Added:
- Source
- ChunkIndex

### Updated Contracts

IRetriever

Old:

Returns:
- DocumentChunk

New:

Returns:
- RetrievalResponse

Containing:
- Results
- Diagnostics

## Retrieval Flow

Question
↓
Chunk Provider
↓
Chunks
↓
Retriever
↓
Scoring
↓
Ranking
↓
Top-K Selection
↓
RetrievalResponse
↓
API Response

## Diagnostics Added

The retriever now exposes:

- TotalChunks
- QualifiedChunks
- ReturnedChunks
- TopK

## Observed Lessons

### Ranking Matters

Two chunks can receive identical scores while having different usefulness.

Example:

Query:
company founded in

Results:

Chunk A:
Nagarro is a digital engineering company

Chunk B:
Nagarro was founded in 1996

Both scored 1.

However, Chunk B is clearly more relevant.

### Equal Keyword Counts Do Not Guarantee Relevance

Keyword frequency alone is insufficient for high-quality retrieval.

### Diagnostics Are Valuable

Visibility into retrieval behavior helps explain why chunks were selected.

## Limitations Still Present

- Exact keyword matching
- No semantic similarity
- No synonym understanding
- No contextual understanding
- No vector search

## Conclusion

Sprint 2 expanded the retrieval engine from simple chunk lookup into a ranked retrieval system with diagnostic capabilities. The observed ranking limitations justify moving toward embeddings and semantic search in future sprints.