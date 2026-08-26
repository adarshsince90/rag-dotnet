# Evaluation Results V1

PDF Extraction:
page.Text

Retrieval Accuracy:
83%

Keyword Coverage:
79%

Observations:

- Source matching overly strict.
- Grounding partially implemented.
- Memory evaluation inconsistent.

---

# Evaluation Results V2

PDF Extraction:
page.GetWords()

Total Questions:
12

Expected Source Coverage:
83.3%

Average Keyword Coverage:
87.5%

Grounding Accuracy:
100%

Average Similarity:
0.67

Average Generation Time:
39.5 seconds

## Key Findings

- Improved PDF extraction significantly improved answer quality.
- Grounding behavior successfully prevented hallucinations.
- Conversational memory successfully handled follow-up questions.
- Retrieval quality improved after re-indexing documents.
- Generation remains the primary performance bottleneck.