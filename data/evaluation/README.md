# Evaluation Framework

## Overview

The evaluation framework validates the quality of the RAG chatbot using a predefined dataset of questions.

The framework executes questions against the production conversational pipeline and measures:

- Retrieval Quality
- Grounding
- Contextual Awareness
- Keyword Coverage
- Response Performance

The same conversation flow used by the UI is used during evaluation.

---

## Run Evaluation

Execute:

POST /evaluation/run

Example:

curl -X POST http://localhost:5000/evaluation/run

---

## Evaluation Dataset

Location:

src/RagDemo.Infrastructure/Evaluation/evaluation-dataset.json

The dataset contains:

- Definition Questions
- Relationship Questions
- Concept Questions
- Paper Questions
- Memory Questions
- Grounding Questions

Example:

{
  "id": 1,
  "category": "definition",
  "conversationId": "eval-1",
  "history": [],
  "question": "What is attention?",
  "expectedSource": "attention-is-what-you-need.pdf",
  "expectedKeywords": [
    "attention",
    "sequence"
  ]
}

---

## Memory Tests

Memory evaluations use:

- History Questions
- ConversationId
- Current Question

Example:

{
  "category": "memory",
  "history": [
    "What is BERT?"
  ],
  "question": "How does it differ from GPT?"
}

The evaluation replays historical questions before executing the final question.

---

## Grounding Tests

Grounding tests verify that the chatbot does not hallucinate answers.

Example:

{
  "category": "grounding",
  "question": "What is human attention?",
  "expectGroundedRefusal": true
}

Expected response:

I could not find the answer in the provided documents.

---

## Metrics

### Expected Source Coverage

Measures whether the expected source PDF was present in the retrieved results.

Formula:

ExpectedSourceCoverage =
ExpectedSourceFoundCount / TotalQuestions

---

### Keyword Coverage

Measures how many expected concepts appeared in the generated answer.

Formula:

KeywordCoverage =
MatchedKeywords / TotalKeywords

Example:

Matched Keywords: 1

Total Keywords: 2

Coverage = 50%

---

### Grounding Accuracy

Measures the chatbot's ability to refuse answering when supporting information is unavailable.

Formula:

GroundingAccuracy =
GroundingPasses / TotalQuestions

---

### Average Similarity Score

Average similarity score of the highest ranked retrieved chunk.

Higher values indicate stronger retrieval confidence.

---

### Generation Time

Average LLM response generation duration in milliseconds.

---

## Current Evaluation Pipeline

Evaluation Dataset
    ↓
EvaluationService
    ↓
ConversationQuestionAnsweringService
    ↓