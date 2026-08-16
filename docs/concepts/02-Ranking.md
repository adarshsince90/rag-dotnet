# Ranking

## What Is Ranking?

Ranking is the process of ordering retrieved chunks according to estimated relevance.

## Sprint 2 Implementation

Keyword Match Count

Score =
Number of matched keywords

## Example

Question:

company founded in

Chunk A:
Nagarro is a digital engineering company

Score = 1

Chunk B:
Nagarro was founded in 1996

Score = 1

Both chunks receive the same score despite having different usefulness.

## Learning

Equal keyword counts do not necessarily indicate equal relevance.

## Future Improvements

- Weighted scoring
- TF-IDF
- BM25
- Embeddings
- Vector similarity
- Hybrid retrieval