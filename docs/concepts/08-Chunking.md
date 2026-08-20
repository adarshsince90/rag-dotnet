# Chunking

## Purpose

Chunking divides large documents into retrievable units.

## Why Chunking Matters

Poor chunking can separate related information.

Example

Chunk A

Nagarro founded in 1996

Chunk B

Headquarters in Germany

Query:

Where is Nagarro based?

Result:

Poor retrieval

## Improved Chunking

Chunk:

Nagarro is a digital engineering company.
Nagarro was founded in 1996.
The headquarters are in Germany.

Result:

Correct retrieval.

## Current Strategy

CharacterChunkingStrategy

Chunk Size:
1000

Overlap:
200

## Future Strategies

- Paragraph Chunking
- Token Chunking
- Semantic Chunking

## Observations

Chunking often impacts retrieval quality more than model selection.

## PDF Chunking

The existing chunking architecture was successfully reused for PDF documents.

Process:

1. Extract text from PDF files.
2. Pass extracted text to configured chunking strategy.
3. Generate DocumentChunk objects.
4. Generate embeddings for chunks.

No changes were required to:

- Retrieval
- Embedding generation
- Prompt building
- Question answering

This validated that chunking was already abstracted correctly.