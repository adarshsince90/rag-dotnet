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