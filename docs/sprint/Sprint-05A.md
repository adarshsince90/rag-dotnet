# Sprint 05A - Chunking Improvements & Retrieval Experiments

## Goal

Improve retrieval quality by introducing configurable chunking strategies and validating retrieval behavior using larger real-world technical documents.

The objective was to understand how chunk size, overlap, similarity thresholds and retrieval configuration influence overall RAG quality before introducing PDF ingestion.

---

## Features Implemented

### Configurable Chunking

Introduced:

json
{
  "Chunking": {
    "ChunkSize": 1000,
    "ChunkOverlap": 200
  }
}


### Similarity Threshold Tuning

Initial threshold:

MinimumSimilarity = 0.60

Observation:

Several relevant chunks were excluded despite
containing useful context.

Revised threshold:

MinimumSimilarity = 0.50

Result:

- Improved retrieval recall
- More relevant chunks reached the LLM
- Higher answer success rate

Learning:

Similarity thresholds are highly corpus-dependent
and should be validated empirically rather than
selected arbitrarily.


### Confidence Gap

Configuration introduced:

ConfidenceGap = 0.036

Status:

Not yet implemented.

Reason:

Experiments revealed that absolute similarity
scores alone may not be sufficient to determine
retrieval confidence.

Future work may evaluate:

Gap =
Rank1Score - Rank2Score

as an additional retrieval confidence signal.

## Sprint 5A Findings

### Finding 1

Chunking improvements had a larger impact
than expected.

### Finding 2

Lowering MinimumSimilarity from 0.60 to 0.50
significantly improved retrieval success.

### Finding 3

Summary-style questions exposed the need
for intent-aware retrieval strategies.

### Finding 4

ConfidenceGap emerged as a potential future
retrieval confidence mechanism but remains
unimplemented.

### Finding 5

The current bottleneck is LLM generation time,
not retrieval performance.
---

## Related

- **ADRs**: [ADR-017 Character Chunking](../adr/ADR-017-character-chunking.md)
- **Concepts**: [Chunking](../concepts/08-Chunking.md)
- **Experiments**: [Chunking Lab](../experiments/chunking-lab.md)
- **Previous Sprint**: [Sprint 04 — LLM Integration](Sprint-04.md)
- **Next Sprint**: [Sprint 05B — PDF Support](Sprint-05B.md)
