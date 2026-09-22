# 06 - Prompt Engineering

# Purpose of This Document

Embeddings and retrieval determine:

```text
What Information Is Retrieved
```

Prompt engineering determines:

```text
How The LLM Uses That Information
```

A RAG system may have:

✅ Excellent retrieval

✅ Excellent embeddings

✅ Excellent vector search

Yet still produce poor answers if prompt construction is weak.

Prompt engineering is the bridge between:

```text
Retrieved Knowledge
```

and

```text
Answer Generation
```

This document explains prompt engineering from first principles and shows how prompts are constructed within our RAG solution.

---

# The Big Picture

The overall RAG pipeline is:

```text
Question
 ↓
Retrieval
 ↓
Relevant Chunks
 ↓
Prompt Construction
 ↓
LLM
 ↓
Answer
```

Retrieval finds information.

Prompt engineering determines how that information is presented to the LLM.

---

# Why Prompt Engineering Matters

Imagine hiring a domain expert.

You provide:

```text
Relevant Documents

Reports

Research Papers

Meeting Notes
```

Without instructions, the expert may:

```text
Ignore Important Details

Make Assumptions

Provide Inconsistent Answers

Speculate Beyond Evidence
```

Now imagine providing clear instructions:

```text
Use only supplied evidence.

Do not make assumptions.

Explain reasoning clearly.

State when information is unavailable.
```

Answer quality improves dramatically.

Prompt engineering serves the same purpose.

---

# What Is A Prompt?

Many people think:

```text
Prompt = Question
```

This is only partially true.

---

# Simple Prompt

```text
What is self-attention?
```

This is technically a prompt.

However, it provides:

```text
No Context

No Instructions

No Constraints
```

---

# RAG Prompt

A RAG prompt contains much more.

Typically:

```text
System Instructions

Conversation History

Retrieved Context

Current Question
```

The LLM receives all of these components together.

---

# Prompt Engineering Definition

> Prompt engineering is the process of designing instructions, context, constraints, and examples that guide an LLM toward producing desired outputs.

---

# Prompt Engineering In Traditional LLM Usage

Basic interaction:

```text
Question
 ↓
LLM
 ↓
Answer
```

The LLM relies entirely on:

```text
Training Data

Pretrained Knowledge
```

---

# Prompt Engineering In RAG

RAG interaction:

```text
Question
 ↓
Retrieve Evidence
 ↓
Build Prompt
 ↓
LLM
 ↓
Grounded Answer
```

Now the model works with:

```text
Retrieved Knowledge

Conversation Context

Instructions
```

instead of relying solely on memory.

---

# Anatomy Of Our Prompt

Our prompt consists of four primary sections:

```text
System Prompt

Conversation History

Retrieved Context

Current Question
```

---

# Visual Representation

```text
+--------------------+
| System Prompt      |
+--------------------+

+--------------------+
| Conversation       |
| History            |
+--------------------+

+--------------------+
| Retrieved Context  |
+--------------------+

+--------------------+
| User Question      |
+--------------------+

          ↓

       Ollama

          ↓

       Response
```

---

# System Prompt

The system prompt is one of the most important parts of prompt engineering.

---

# Purpose

The system prompt defines:

```text
Role

Behavior

Rules

Constraints

Tone
```

before the model processes any question.

---

# Example

```text
You are a helpful AI assistant.

Answer using only the supplied context.

If information is unavailable,
say you do not know.

Do not invent information.
```

---

# Why System Prompts Exist

Without a system prompt:

```text
The Model Decides The Rules
```

With a system prompt:

```text
We Define The Rules
```

---

# System Prompt Responsibilities

Typical responsibilities include:

```text
Grounding Instructions

Answer Style

Safety Constraints

Formatting Requirements

Output Structure
```

---

# Assessment Question

## Why Is A System Prompt Needed?

**Answer**

A system prompt establishes the behavioral guidelines and constraints that govern how the model should answer questions.

---

# Context Injection

After retrieval, the relevant chunks are added to the prompt.

---

Example:

```text
Chunk 1:
Self-attention is a mechanism...

Chunk 2:
Self-attention enables...

Chunk 3:
The Transformer architecture...
```

---

These chunks become:

```text
Context:
...
```

inside the prompt.

---

# Important Principle

The LLM does NOT interact with:

```text
Qdrant

Vector Search

Embeddings
```

directly.

The LLM only sees:

```text
Plain Text
```

inside the prompt.

---

# Retrieval And Prompting Relationship

Retrieval produces:

```text
Evidence
```

Prompt construction packages that evidence for the model.

---

Flow:

```text
Qdrant
 ↓
Retrieved Chunks
 ↓
Prompt Builder
 ↓
LLM
```

---

# Conversation History Injection

One of the most important RAG enhancements.

---

# Example

User:

```text
What is self-attention?
```

Assistant:

```text
...
```

User:

```text
What are its advantages?
```

---

Without conversation history:

```text
its = ?
```

Ambiguous.

---

With conversation history:

```text
The model understands:

its = self-attention
```

---

# Purpose Of Conversation History

History enables:

```text
Follow-Up Questions

Context Awareness

Multi-Turn Conversations

Natural Interactions
```

---

# Example Prompt Fragment

```text
Conversation History

User:
What is self-attention?

Assistant:
Self-attention is...

Current Question:
What are its advantages?
```

---

# Current User Question

The final section is the user query.

Example:

```text
Question:
What are the advantages of self-attention?
```

Although this appears small, it gives purpose to everything else in the prompt.

---

# Complete Prompt Example

A simplified example:

```text
System:

You are a helpful assistant.

Answer only using the supplied context.

If information is unavailable,
say you do not know.

-----------------------------------

Conversation History:

User:
What is self-attention?

Assistant:
...

-----------------------------------

Context:

Chunk 1 ...

Chunk 2 ...

Chunk 3 ...

-----------------------------------

Question:

What are its advantages?
```

This closely resembles the prompt construction process used by production RAG systems.

---

# Why Prompt Engineering Reduces Hallucinations

Without retrieval:

```text
Question
 ↓
Model Memory
 ↓
Answer
```

Potentially:

```text
Hallucination
```

---

With prompt engineering:

```text
Question
 ↓
Retrieved Evidence
 ↓
Instructions
 ↓
Answer
```

The model is guided toward evidence-based responses.

---

# Grounding Instructions

Grounding is one of the most important prompt engineering concepts.

Example:

```text
Answer only using the provided context.

Do not invent information.

If evidence is unavailable,
state that clearly.
```

---

# Why Grounding Matters

Grounding:

```text
Reduces Hallucination

Improves Trustworthiness

Improves Consistency

Improves Explainability
```

---

# Prompt Engineering Patterns

---

# Pattern 1 - Grounded Answering

Example:

```text
Use only the supplied context.
```

Purpose:

```text
Reduce Hallucinations
```

---

# Pattern 2 - Refusal Pattern

Example:

```text
If the information is not present,
say you do not know.
```

Purpose:

```text
Prevent Fabrication
```

---

# Pattern 3 - Structured Output

Instead of:

```text
Freeform Text
```

Request:

```text
Summary

Key Findings

Recommendations
```

or:

```json
{
  "summary": "",
  "findings": []
}
```

---

# Benefits

```text
Predictable Responses

Machine Readability

Easier Processing
```

---

# Pattern 4 - Source-Aware Responses

Example:

```text
Reference supplied sources when possible.
```

Benefits:

```text
Transparency

Trust

Traceability
```

---

# Pattern 5 - Step-By-Step Reasoning

Example:

```text
Explain your reasoning step-by-step.
```

Improves:

```text
Complex Problem Solving
```

---

# Prompt Templates

Prompt templates separate:

```text
Prompt Structure
```

from

```text
Dynamic Data
```

---

# Example

Template:

```text
{SystemPrompt}

{ConversationHistory}

{RetrievedContext}

Question:
{Question}
```

---

Benefits:

```text
Reusability

Consistency

Maintainability
```

---

# Why Templates Matter

Without templates:

```text
String Concatenation Everywhere
```

---

With templates:

```text
Centralized Design

Easy Updates

Cleaner Architecture
```

---

# Context Window Limitations

Every LLM has a maximum context window.

Examples:

```text
32K Tokens

64K Tokens

128K Tokens

200K+ Tokens
```

---

# Why This Matters

Too much context:

```text
Prompt Too Large
```

causes:

```text
Higher Cost

Higher Latency

Possible Truncation
```

---

# Prompt Engineering Trade-Off

Balance:

```text
Context Coverage

Prompt Size

Retrieval Quality
```

---

# More Context Is Not Always Better

Common misconception:

```text
More Chunks
=
Better Answers
```

False.

---

Too much context can introduce:

```text
Noise

Conflicting Information

Reduced Focus
```

---

# Common Prompt Engineering Mistakes

---

# Weak Instructions

Example:

```text
Answer the question.
```

Problem:

```text
Too Vague
```

---

# Missing Grounding

Example:

```text
No constraints provided.
```

Result:

```text
Higher Hallucination Risk
```

---

# Excessive Context

Example:

```text
20+ Large Chunks
```

Result:

```text
Signal Lost In Noise
```

---

# Contradictory Instructions

Example:

```text
Be concise.

Provide exhaustive detail.
```

Mixed instructions confuse the model.

---

# Ignoring Conversation History

Result:

```text
Poor Follow-Up Responses
```

---

# Missing Refusal Instructions

Without:

```text
Say you do not know.
```

The model is more likely to fabricate information.

---

# Prompt Quality vs Model Quality

A useful principle:

```text
Good Prompt
+
Smaller Model
```

often outperforms:

```text
Bad Prompt
+
Larger Model
```

Poor prompts waste powerful models.

---

# What Our Implementation Does

Our RAG solution constructs prompts using:

```text
System Instructions

Conversation History

Retrieved Chunks

Current Question
```

The prompt is then sent to:

```text
Ollama
```

for answer generation.

---

# Current Flow

```text
Question
 ↓
Embedding
 ↓
Qdrant Retrieval
 ↓
Top-K Chunks
 ↓
Prompt Construction
 ↓
Ollama
 ↓
Streaming Response
```

---

# Potential Future Enhancements

---

# Query Rewriting

Current:

```text
Question
 ↓
Retrieve
```

Future:

```text
Question
 ↓
Rewrite Question
 ↓
Retrieve
```

---

# Dynamic Context Selection

Current:

```text
Fixed Retrieval Strategy
```

Future:

```text
Simple Question → Small Context

Complex Question → Larger Context
```

---

# Multi-Step Prompting

Example:

```text
Retrieve
 ↓
Analyze
 ↓
Retrieve Again
 ↓
Answer
```

Often seen in advanced or agentic RAG systems.

---

# Common Assessment Questions

## What Is Prompt Engineering?

> Prompt engineering is the process of designing instructions, context, and constraints that guide an LLM toward producing desired outputs.

---

## Why Is Prompt Engineering Important?

> The quality of prompts directly influences answer quality, grounding, consistency, and hallucination reduction.

---

## What Is A System Prompt?

> A system prompt defines the behavior, role, and constraints the model should follow throughout a conversation.

---

## Why Include Retrieved Context?

> Retrieved context provides evidence-based information that allows the model to generate grounded answers.

---

## Why Include Conversation History?

> Conversation history helps the model understand follow-up questions and maintain context across multiple interactions.

---

## How Does Prompt Engineering Reduce Hallucination?

> Prompt engineering supplies relevant evidence and explicit grounding instructions, reducing the need for the model to guess.

---

## Why Not Send Only The Question?

> Without context and instructions, the model relies solely on its training knowledge and may generate inaccurate or ungrounded answers.

---

## Why Use Prompt Templates?

> Prompt templates provide consistent structure, improve maintainability, and simplify prompt construction.

---

# Key Takeaways

```text
Prompts Are More Than Questions.

System Prompts Define Behavior.

Retrieved Context Provides Evidence.

Conversation History Enables Multi-Turn Understanding.

Prompt Templates Improve Consistency.

Grounding Instructions Reduce Hallucinations.

Prompt Engineering Connects Retrieval To Generation.

Good Retrieval + Good Prompting = Better Answers.
```

---

# 60-Second Assessment Answer

> "Prompt engineering is the process of constructing the instructions and context supplied to the LLM. In our RAG solution, the prompt contains four primary components: system instructions, conversation history, retrieved document context, and the current user question. The system prompt establishes behavior and grounding rules, the retrieved chunks provide evidence, conversation history preserves context across multiple turns, and the current question defines the task. Effective prompt engineering helps reduce hallucinations, improves consistency, ensures retrieved information is used correctly, and significantly improves the quality of generated answers."