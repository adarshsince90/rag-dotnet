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