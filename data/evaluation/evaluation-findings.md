Results

---

Run 1:

Dataset Size: 12 Questions

Retrieval Accuracy: 75%

Keyword Accuracy: 91.7%

Average Keyword Coverage: 95.8%

Average Similarity Score: 0.64

Average Generation Time: 49.3 seconds

Key Observations

- Retrieval quality was generally strong.
- Conversational memory tests passed successfully.
- Grounding behavior was observed for out-of-scope questions.
- Keyword matching is useful but imperfect for semantic evaluation.
- Generation latency remains significantly higher than retrieval latency.

---

Run 2:

# Evaluation Methodology

## Objective

Evaluate retrieval quality, grounding, conversational memory,
and response relevance of the RAG chatbot.

## Dataset

12 evaluation scenarios:

- Definitions
- Relationships
- Concepts
- Paper Questions
- Memory Tests
- Grounding Tests

## Metrics

### Expected Source Coverage

Measures whether the expected source PDF was included
within retrieved sources.

### Keyword Coverage

Measures proportion of expected concepts present
in generated answers.

Formula:

KeywordCoverage =
MatchedKeywords / TotalKeywords

### Grounding Accuracy

Measures ability to refuse answering when
requested information is unavailable.

### Contextual Awareness

Evaluated using memory-based conversations.

## Evaluation Pipeline

Dataset
→ ConversationQuestionAnsweringService
→ Retrieval
→ Prompt Generation
→ LLM
→ Evaluation