# Sprint 01 - Retrieval Fundamentals

## Goal

Build a simple retrieval pipeline and understand retrieval concepts before introducing embeddings, vector databases, and language models.

## Features Implemented

- Text file ingestion
- Basic chunk generation
- Keyword-based retrieval
- Question normalization
- Punctuation removal
- Stop-word filtering
- Retrieval scoring
- Ask endpoint
- End-to-end retrieval flow

## Architecture

User Question
↓
API Endpoint
↓
AskQuestionService
↓
IChunkProvider
↓
TextFileChunkProvider
↓
IRetriever
↓
KeywordRetriever
↓
Retrieval Result (Chunk + Score)
↓
Response

## Learning Outcomes

- Retrieval and generation are separate concerns.
- Retrieval quality directly impacts answer quality.
- Dependency inversion enables implementation replacement without affecting workflows.
- Simple keyword search works for obvious matches but struggles with semantic understanding.

## Enhancements Added

### Question Normalization

Removes punctuation and normalizes text before matching.

Example:

headquarter?
→
headquarter

### Stop Word Removal

Ignores low-value words such as:

- what
- where
- is
- are
- the
- a
- an

### Retrieval Score

Retrieval now exposes scoring information to help explain why a chunk was selected.

## Observed Limitations

- No semantic understanding
- No synonym handling
- No contextual reasoning
- No typo tolerance
- No ranking beyond simple keyword matching

## Examples

Question:
Where is Nagarro headquartered?

Expected:
Germany

Question:
Where is Nagarro based?

Result:
May fail because keyword retrieval cannot infer that "based" and "headquarters" are related concepts.

## Conclusion

The system successfully retrieves relevant content using keyword matching and demonstrates the need for semantic retrieval techniques such as embeddings and vector similarity search.


Sprint 1 established the foundational retrieval pipeline and validated the project's architecture. Although simple keyword retrieval produced acceptable results for direct matches, the observed limitations around semantic understanding demonstrated the need for embeddings and vector search. This sprint successfully clarified the distinction between retrieval and generation, which is a core concept for understanding modern RAG systems.