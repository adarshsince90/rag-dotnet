# Retrieval

## What Is Retrieval?

Retrieval is the process of finding the most relevant information for a query.

## Sprint 1 Implementation

Keyword Retrieval

Process:

Question
↓
Normalization
↓
Stop Word Removal
↓
Chunk Comparison
↓
Scoring
↓
Best Match

## Advantages

- Simple
- Explainable
- Fast
- Easy to implement

## Disadvantages

- No semantic understanding
- No synonym support
- Sensitive to phrasing
- Sensitive to vocabulary differences

## Why Embeddings Are Needed

Keyword retrieval compares words.

Embeddings compare meaning.

Example:

headquarters

and

based

have related meaning but different words.

Keyword retrieval struggles with this.

Embedding-based retrieval solves this limitation.


## Sprint 2 Implementation

## Ranking

Retrieval is not only about finding matches.

Retrieval systems must rank candidate chunks according to relevance.

Current implementation:

Keyword score

Future implementation:

Vector similarity score

---

## Top-K Retrieval

Instead of returning a single chunk, modern retrieval systems return multiple ranked chunks.

Benefits:

- More context
- Better answer generation
- Improved recall

Current strategy:

Top 3 chunks

---

## Retrieval Diagnostics

Retrieval diagnostics provide visibility into:

- Number of chunks processed
- Number of matching chunks
- Number of results returned

Diagnostics help explain retrieval behavior and support troubleshooting.


## Retrieval Evolution

Sprint 1

Keyword Retrieval

↓

Sprint 3

Semantic Retrieval

---

## Retrieval Types

### Keyword Retrieval

Pros:

- Exact matching
- Entity matching

Cons:

- No semantic understanding

---

### Vector Retrieval

Pros:

- Semantic understanding
- Synonym handling

Cons:

- Can struggle with entity-heavy queries

---

### Hybrid Retrieval

Combines:

- Keyword Retrieval
- Vector Retrieval

to leverage strengths of both approaches.

---

## Retrieval Types

Keyword Retrieval

- Exact matching
- Entity matching

Vector Retrieval

- Semantic similarity
- Meaning-based retrieval

Future

Hybrid Retrieval

Keyword Score
+
Vector Score

## Current Retrieval Configuration

TopK = 3

MinimumSimilarity = 0.50

## Future Enhancements

### Dynamic TopK

Fact Question

TopK = 3

Explanation Question

TopK = 5

Summary Question

TopK = 10

### Query Classification

Question intent determines retrieval strategy.