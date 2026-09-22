# 09 - Evaluation And Grounding

# Purpose of This Document

Building a RAG system is not enough.

A system that can generate answers is useful.

A system that can prove answer quality is valuable.

Many RAG implementations stop at:

```text
Question
 ↓
Answer
```

and rely on:

```text
"Looks good to me."
```

However, engineering requires:

```text
Measurement

Validation

Repeatability

Evidence
```

This document explains:

- Why evaluation is necessary
- What grounding means
- How hallucinations occur
- How to measure retrieval quality
- How to evaluate generated answers
- Evaluation strategies
- Diagnostics and observability
- Execution evidence
- How our implementation approaches quality measurement

---

# Why Evaluation Exists

Traditional software systems are usually deterministic.

Given:

```text
Input A
```

you expect:

```text
Output A
```

every time.

Example:

```text
2 + 2 = 4
```

No ambiguity.

---

LLM systems are different.

Given:

```text
Question:
Explain self-attention.
```

The model may produce:

```text
Answer Version 1
```

or

```text
Answer Version 2
```

or

```text
Answer Version 3
```

All may be valid.

---

This creates a challenge:

```text
How do we measure quality?
```

Without evaluation:

```text
We Guess.
```

With evaluation:

```text
We Measure.
```

---

# The Fundamental Principle

A RAG system should not be judged solely by:

```text
How Intelligent The Answer Sounds
```

Instead it should be judged by:

```text
How Correct

How Grounded

How Relevant

How Reliable
```

the answer is.

---

# What Does "Good Answer" Mean?

A good answer typically has several properties:

```text
Correct

Relevant

Grounded

Complete

Clear

Trustworthy
```

---

Example:

Question:

```text
What is self-attention?
```

Answer:

```text
Self-attention is a mechanism that allows
tokens to attend to other tokens in a sequence.
```

May be:

```text
Correct
```

---

Question:

```text
What are the findings in this PDF?
```

Answer:

```text
The paper recommends reinforcement learning.
```

If reinforcement learning is not present in the document:

```text
Incorrect
```

even if it sounds convincing.

---

# The Four Evaluation Layers

A useful mental model is to evaluate RAG systems across four layers.

---

# Layer 1 - Retrieval Quality

Question:

```text
Did we retrieve the correct chunks?
```

---

# Layer 2 - Context Quality

Question:

```text
Did we retrieve enough information?
```

---

# Layer 3 - Answer Quality

Question:

```text
Did the model answer correctly?
```

---

# Layer 4 - System Quality

Question:

```text
Was the system efficient?
```

Examples:

```text
Latency

Token Usage

Generation Time

User Experience
```

---

# Evaluation Pipeline

```text
Question
 ↓

Retrieve Chunks
 ↓

Evaluate Retrieval
 ↓

Generate Answer
 ↓

Evaluate Answer
 ↓

Analyze Metrics
```

---

# What Is Grounding?

Grounding is one of the most important RAG concepts.

---

# Definition

> A grounded answer is an answer that is supported by retrieved evidence.

---

# Example Of Grounded Answer

Retrieved Context:

```text
The Transformer architecture uses
self-attention instead of recurrence.
```

Answer:

```text
The Transformer uses self-attention
instead of recurrence.
```

Supported by evidence.

Therefore:

```text
Grounded
```

---

# Example Of Ungrounded Answer

Retrieved Context:

```text
The Transformer uses self-attention.
```

Answer:

```text
The Transformer was introduced in 2019.
```

The retrieved context does not support this claim.

Therefore:

```text
Ungrounded
```

---

# Simple Grounding Rule

Ask:

```text
Can I point to retrieved evidence
that supports this statement?
```

---

If:

```text
Yes
```

The answer is likely grounded.

---

If:

```text
No
```

The answer may contain hallucinations.

---

# Why Grounding Matters

Grounding helps:

```text
Reduce Hallucinations

Improve Trust

Increase Explainability

Improve Reliability
```

---

Grounding is one of the primary reasons RAG exists.

---

# What Is A Hallucination?

A hallucination occurs when a model generates information that is:

```text
Incorrect

Invented

Unsupported

Misleading
```

---

# Example

Question:

```text
What does section 12 say?
```

Retrieved Context:

```text
Sections 1-10
```

Answer:

```text
Section 12 discusses compliance requirements.
```

If section 12 was never retrieved:

```text
Potential Hallucination
```

---

# Hallucination Types

---

## Type 1 - Invented Facts

Example:

```text
Stating information that does not exist.
```

---

## Type 2 - Unsupported Conclusions

Example:

```text
Jumping beyond available evidence.
```

---

## Type 3 - False Attribution

Example:

```text
Claiming a document stated something
it never actually stated.
```

---

# Relationship Between RAG And Hallucination

Without RAG:

```text
Question
 ↓
Model Memory
 ↓
Answer
```

The model relies entirely on training data.

---

With RAG:

```text
Question
 ↓
Retrieve Evidence
 ↓
Generate Answer
```

The model has supporting information.

---

Hallucinations are not eliminated.

But they are significantly reduced.

---

# Evaluating Retrieval Quality

Before evaluating answers, we must evaluate retrieval.

---

# Why Retrieval Quality Matters

The LLM can only answer from the context it receives.

---

Bad Retrieval:

```text
Wrong Chunks
```

↓

```text
Wrong Context
```

↓

```text
Wrong Answer
```

---

Even the best model cannot fix missing evidence.

---

# Retrieval Evaluation Questions

Ask:

```text
Did we retrieve relevant chunks?

Did we retrieve enough chunks?

Did we retrieve too many chunks?

Did retrieval miss important information?
```

---

# Retrieval Relevance

Question:

```text
How useful were the retrieved chunks?
```

---

Good Retrieval:

```text
Relevant

Focused

Related To Question
```

---

Poor Retrieval:

```text
Irrelevant

Noisy

Unrelated
```

---

# Retrieval Coverage

Question:

```text
Did retrieval cover all required information?
```

---

Example:

Question:

```text
Compare RNNs and Transformers.
```

---

Retrieved:

```text
Transformer Chunks Only
```

---

Coverage:

```text
Incomplete
```

---

# Precision

Simple definition:

```text
Of Retrieved Chunks,
How Many Were Relevant?
```

---

High Precision:

```text
Less Noise
```

---

Low Precision:

```text
Many Irrelevant Chunks
```

---

# Recall

Simple definition:

```text
Did We Retrieve Everything Important?
```

---

High Recall:

```text
Most Relevant Information Retrieved
```

---

Low Recall:

```text
Important Context Missing
```

---

# Precision vs Recall Trade-Off

Small Top-K:

```text
High Precision

Low Recall
```

---

Large Top-K:

```text
Higher Recall

Potentially Lower Precision
```

---

Balancing both is important.

---

# Evaluating Answer Quality

Even if retrieval succeeds:

```text
Answer Quality
```

must still be evaluated.

---

# Core Dimensions

---

## Correctness

Question:

```text
Is The Answer Factually Correct?
```

---

## Groundedness

Question:

```text
Does Retrieved Evidence Support It?
```

---

## Completeness

Question:

```text
Did The Answer Cover Important Points?
```

---

## Clarity

Question:

```text
Is The Answer Understandable?
```

---

## Relevance

Question:

```text
Did It Answer The Actual Question?
```

---

# Example Evaluation Checklist

Question:

```text
What is self-attention?
```

Evaluate:

```text
Correct?
Grounded?
Complete?
Relevant?
Clear?
```

---

# Human Evaluation

The simplest evaluation approach.

---

Process:

```text
Human Reads Question

Human Reads Answer

Human Scores Quality
```

---

Advantages:

```text
Flexible

Accurate

Context Aware
```

---

Disadvantages:

```text
Expensive

Slow

Difficult To Scale
```

---

# Automated Evaluation

As systems grow:

```text
Manual Evaluation
```

becomes insufficient.

---

Automation helps measure quality consistently.

---

# Automated Approaches

Examples:

```text
Rule-Based Evaluation

Reference Answer Comparison

LLM-As-Judge

Scorecards
```

---

# Reference Answer Evaluation

Create:

```text
Expected Answer
```

for a question.

---

Compare:

```text
Expected Answer

Generated Answer
```

to assess quality.

---

Advantages:

```text
Repeatable

Consistent
```

---

Challenges:

```text
Many Correct Answers May Exist
```

---

# LLM-As-Judge

One of the most popular modern approaches.

---

Workflow:

```text
Question

Retrieved Context

Generated Answer

Evaluation Criteria
```

↓

```text
Evaluation Model
```

↓

```text
Score
```

---

Example Evaluation Dimensions

```text
Correctness

Grounding

Completeness

Relevance
```

---

# Why LLM-As-Judge Is Useful

Benefits:

```text
Scalable

Consistent

Automated
```

---

Limitations:

```text
Judges Can Be Wrong

Evaluation Bias

Prompt Sensitivity
```

---

# Grounding Evaluation

One particularly important evaluation.

---

Question:

```text
Can Every Major Claim Be Supported
By Retrieved Evidence?
```

---

Example

Evidence:

```text
Transformer uses self-attention.
```

Answer:

```text
Transformer uses self-attention.
```

Grounded.

---

Answer:

```text
Transformer introduced reinforcement learning.
```

Not grounded.

---

# System-Level Evaluation

Beyond answer quality, evaluate:

```text
Performance

Scalability

User Experience
```

---

Examples:

```text
Retrieval Time

Generation Time

End-To-End Latency

Token Usage
```

---

# Diagnostics

Diagnostics help explain system behavior.

---

Without diagnostics:

```text
Answer Looks Wrong
```

but:

```text
Why?
```

Unknown.

---

With diagnostics:

```text
Retrieved Chunks

Similarity Scores

Sources

Latency

Generation Metrics
```

we can investigate issues.

---

# Diagnostics In Our Solution

Examples include:

```text
Retrieved Sources

Similarity Scores

Chunk Counts

Retrieval Duration

Generation Duration

Conversation Information
```

These improve observability and troubleshooting.

---

# Execution Evidence

One piece of reviewer feedback highlighted:

```text
Artifacts Provide Execution Evidence
```

This is an important engineering practice.

---

# What Is Execution Evidence?

Evidence demonstrating:

```text
What Was Run

What Was Measured

What Result Was Produced
```

---

Examples:

```text
Evaluation Reports

Metrics

Screenshots

Logs

Generated Outputs

Experiments
```

---

# Why Execution Evidence Matters

Without evidence:

```text
Claims
```

---

With evidence:

```text
Proof
```

---

Engineering decisions should be supported by data.

---

# Failure Analysis

Good evaluations help identify failures.

---

# Scenario 1

Good Retrieval

Poor Prompt

↓

```text
Poor Answer
```

---

# Scenario 2

Poor Retrieval

Excellent Prompt

↓

```text
Poor Answer
```

---

# Scenario 3

Poor Retrieval

Poor Prompt

↓

```text
Poor Answer
```

---

# Scenario 4

Good Retrieval

Good Prompt

↓

```text
Strong Grounded Answer
```

---

# Evaluation Maturity Model

---

# Level 0

```text
Looks Good To Me
```

No measurement.

---

# Level 1

```text
Manual Testing
```

Occasional validation.

---

# Level 2

```text
Evaluation Dataset
```

Repeatable questions.

---

# Level 3

```text
Automated Scoring
```

Continuous measurement.

---

# Level 4

```text
Continuous Evaluation Pipeline
```

Quality checked automatically.

---

Our project moved beyond:

```text
Level 0

and

Level 1
```

through repeatable evaluations, diagnostics, and artifacts.

---

# Our Evaluation Approach

The solution includes:

```text
Retrieved Sources

Retrieval Diagnostics

Grounding Awareness

Execution Reports

Evaluation Artifacts

Generation Metadata
```

---

Evaluation focuses on:

```text
Retrieval Quality

Answer Quality

Grounding

System Metrics
```

rather than relying solely on subjective observations.

---

# Common Assessment Questions

## Why Is Evaluation Important?

> Evaluation provides objective evidence that a system is producing useful, reliable, and grounded answers.

---

## What Is Grounding?

> Grounding means answering questions using retrieved evidence rather than unsupported assumptions.

---

## What Is A Hallucination?

> A hallucination occurs when a model generates information that is unsupported, incorrect, or fabricated.

---

## Why Evaluate Retrieval?

> Retrieval determines the context supplied to the model. Poor retrieval often leads directly to poor answers.

---

## What Is Retrieval Precision?

> Precision measures how many retrieved chunks were actually relevant.

---

## What Is Retrieval Recall?

> Recall measures whether important information was successfully retrieved.

---

## What Is LLM-As-Judge?

> LLM-As-Judge uses an LLM to assess generated answers against evaluation criteria such as correctness, grounding, and completeness.

---

## Why Are Diagnostics Useful?

> Diagnostics help explain system behavior and identify causes of poor answers or performance issues.

---

## Why Are Evaluation Artifacts Important?

> Artifacts provide measurable evidence that supports quality claims and engineering decisions.

---

# Key Takeaways

```text
You Cannot Improve What You Do Not Measure.

Grounding Reduces Hallucinations.

Retrieval Quality Directly Affects Answer Quality.

Evaluation Must Cover Retrieval And Generation.

Diagnostics Explain Why Systems Behave The Way They Do.

Execution Evidence Builds Trust.

Good Engineering Requires Measurement, Not Assumptions.

Quality Should Be Demonstrated Through Metrics And Artifacts.
```

---

# 60-Second Assessment Answer

> "Evaluation ensures that a RAG system is producing reliable, grounded responses rather than simply generating plausible text. A strong evaluation strategy examines retrieval quality, answer quality, grounding, and overall system performance. Grounding measures whether answers are supported by retrieved evidence and is one of the primary techniques used to reduce hallucinations. Our solution includes diagnostics, source attribution, retrieval metadata, evaluation artifacts, and execution evidence that allow us to objectively assess system behavior and compare improvements over time. This transforms the system from a proof-of-concept into a measurable engineering solution."