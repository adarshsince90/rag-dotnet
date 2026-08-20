# Chunking Laboratory

## Dataset

- transformers-llm.txt
- attention-is-all-you-need.txt

## Configuration

Chunk Size:
1000

Overlap:
200

TopK:
3

MinimumSimilarity:
0.50

## Successful Questions

### What is BERT?

✅ Correct

### What is Attention?

✅ Correct

### Explain Attention and Transformers

✅ Correct

## Misspelling Tests

✅ BERTs

✅ BERTing

✅ transfomers

❌ BERTH

## Key Findings

- Chunking quality significantly impacts retrieval quality.
- Larger context improved semantic retrieval.
- Query intent affects retrieval requirements.
- Current configuration works well for technical documentation.

---
## PDF Corpus Validation

The chunking strategies were validated against
real-world transformer and LLM research papers.

Findings:

- Character chunking with overlap performed reliably.
- Larger chunks improved retrieval quality.
- Lowering MinimumSimilarity to 0.50 significantly improved recall.
- Technical research papers produced meaningful semantic matches.
- Retrieval remained fast despite increased corpus size.

The primary scalability challenge shifted from retrieval
to embedding generation throughput.

## PDF Document Pipeline

Sprint 5B introduced PDF document ingestion to replace the previous text-file-only workflow.

Current ingestion flow:

PDF Documents
    ↓
PdfChunkProvider
    ↓
PdfDocumentExtractor
    ↓
Extracted Text
    ↓
IChunkingStrategy
    ↓
Document Chunks
    ↓
GenerateEmbeddingsService
    ↓
IChunkStore
    ↓
Vector Retrieval

The retrieval, prompting, and LLM layers remain unchanged.

This demonstrates the benefit of separation of concerns:
only the document acquisition layer changed while the
retrieval and generation pipeline remained stable.

## Large Corpus Validation

Using five transformer-related research papers:

- Attention Is All You Need
- BERT Pretraining
- Language Models Are Few-Shot Learners
- Transformer Limitations
- Transformers and LLMs

Results:

- 629 total chunks generated
- Retrieval remained under one second
- Semantic matches remained highly relevant
- Technical concept questions produced grounded answers

Observation:

Increasing corpus size did not significantly impact retrieval latency.

The primary scalability concern shifted from retrieval to embedding generation.
