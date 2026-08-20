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