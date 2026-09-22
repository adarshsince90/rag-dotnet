# 13 - Design Decisions, Trade-Offs And Lessons Learned

# Purpose of This Document

Understanding how a system works is important.

Understanding **why it was designed that way** is what separates implementation knowledge from architectural thinking.

Every technology choice carries:

```text
Benefits

Costs

Risks

Trade-Offs
```

There are rarely perfect decisions in software architecture.

Instead, architecture is the process of selecting the most appropriate solution given:

```text
Requirements

Constraints

Team Skills

Timeline

Budget

Future Goals
```

This document captures the major design decisions made throughout our RAG journey, the alternatives that were considered, the trade-offs accepted, lessons learned, and future evolution paths.

---

# The Fundamental Truth About Architecture

One of the most important lessons for engineers:

```text
There Is No Best Technology

Only The Best Technology
For A Particular Context
```

---

Example:

```text
Qdrant
```

is not universally better than:

```text
Pinecone
```

nor is:

```text
Ollama
```

universally better than:

```text
OpenAI
```

The correct answer is always:

```text
It Depends
```

on requirements and constraints.

---

# Our Decision Framework

Throughout the project, decisions followed a simple pattern:

```text
Problem
      ↓

Possible Options
      ↓

Trade-Off Analysis
      ↓

Decision
      ↓

Future Implications
```

---

# Decision 1 - Why Build A RAG System?

# Problem

The application needed to answer questions using information contained within uploaded documents.

---

# Alternatives

Option 1:

```text
Pure LLM
```

---

Option 2:

```text
Fine-Tuned Model
```

---

Option 3:

```text
Retrieval Augmented Generation
```

---

# Why Pure LLM Was Not Enough

Limitations:

```text
No Access To Uploaded Documents

Knowledge Cutoff

Cannot Access Private Information

Higher Hallucination Risk
```

---

# Why Fine-Tuning Was Not Ideal

Advantages:

```text
Knowledge Embedded Into Model
```

---

Disadvantages:

```text
Expensive

Slow To Update

Requires Retraining

Difficult Maintenance
```

---

# Why RAG Was Chosen

Benefits:

```text
Dynamic Knowledge

Easier Updates

Lower Cost

Source Attribution

Reduced Hallucination
```

---

# Trade-Off Accepted

```text
Additional Retrieval Complexity
```

was accepted in exchange for:

```text
Knowledge Freshness
```

---

# Lesson Learned

```text
Knowledge Changes Faster Than Models
```

RAG keeps knowledge external and easier to maintain.

---

# Decision 2 - Why .NET 10?

# Problem

Need a backend platform for:

```text
APIs

Streaming

Dependency Injection

Observability

Service Architecture
```

---

# Alternatives

```text
.NET

Python

Node.js

Java
```

---

# Why .NET Was Chosen

Benefits:

```text
Strong Existing Expertise

Excellent Performance

Dependency Injection

Minimal APIs

Clean Architecture Support

Strong Typing

Enterprise Ecosystem
```

---

# Trade-Offs

Compared to Python:

```text
Smaller AI Ecosystem

Fewer AI Tutorials

Less Community Samples
```

---

# Why The Trade-Off Was Acceptable

The goal was not:

```text
Learning Python Frameworks
```

The goal was:

```text
Understanding AI Architecture
```

---

# Lesson Learned

Framework popularity is less important than strong engineering fundamentals.

---

# Decision 3 - Why Clean Architecture?

# Problem

The solution contains:

```text
Retrieval

Prompt Engineering

Memory

Evaluation

Streaming

Models
```

Complexity grows quickly.

---

# Alternative

Simple layered architecture.

---

# Why Clean Architecture Was Chosen

Benefits:

```text
Separation Of Concerns

Testability

Maintainability

Replaceable Components

Independent Business Logic
```

---

# Example

Swap:

```text
Qdrant
```

with:

```text
Pinecone
```

without altering business logic.

---

# Trade-Offs

```text
More Files

More Abstractions

Higher Initial Complexity
```

---

# Lesson Learned

Small projects benefit little from architecture.

Large projects benefit enormously.

---

# Decision 4 - Why Ollama?

# Problem

Need local LLM execution.

---

# Alternatives

```text
OpenAI

Azure OpenAI

Anthropic

Ollama
```

---

# Why Ollama Was Chosen

Benefits:

```text
Local Execution

No Per-Token Cost

Privacy

Offline Development

Experimentation Freedom
```

---

# Trade-Offs

```text
Infrastructure Ownership

Local Resource Usage

Potential Quality Differences
```

---

# Lesson Learned

Owning infrastructure increases operational responsibility but drastically increases flexibility.

---

# Decision 5 - Why Qdrant?

One of the most important architectural decisions.

---

# Alternatives

```text
Qdrant

Pinecone

pgvector

Weaviate

Azure AI Search
```

---

# Why Qdrant Was Chosen

Benefits:

```text
Open Source

Easy Local Development

Excellent Vector Search

Rich Filtering

Strong Documentation

Container Friendly
```

---

# Why Not Pinecone?

Pros:

```text
Managed Service

Reduced Operations
```

Cons:

```text
Ongoing Cost

External Dependency
```

---

# Why Not pgvector?

Pros:

```text
Runs In PostgreSQL

Simple Infrastructure
```

Cons:

```text
Vector Search Is Not Primary Focus
```

---

# Why Not Azure AI Search?

Pros:

```text
Managed Enterprise Service
```

Cons:

```text
Cloud Dependency

Additional Cost
```

---

# Lesson Learned

For learning and experimentation:

```text
Open Source + Local Deployment
```

provides maximum control.

---

# Decision 6 - Why Embeddings Instead Of Keywords?

# Problem

Keyword search struggles with semantic meaning.

---

Example

Question:

```text
What are the advantages of attention mechanisms?
```

Document:

```text
Benefits of self-attention...
```

Keyword matching may fail.

---

# Why Embeddings Were Chosen

Benefits:

```text
Semantic Search

Language Understanding

Meaning-Based Retrieval
```

---

# Trade-Offs

```text
Additional Computation

Embedding Costs

More Complex Infrastructure
```

---

# Lesson Learned

Semantic understanding is the foundation of modern retrieval systems.

---

# Decision 7 - Why Chunking?

# Problem

Entire documents are too large.

---

Without chunking:

```text
One Document

One Embedding
```

---

Problems:

```text
Poor Precision

Mixed Topics

Inefficient Retrieval
```

---

# Chunking Benefits

```text
Granular Retrieval

Better Precision

Reduced Noise
```

---

# Trade-Offs

```text
More Embeddings

Storage Growth

Chunking Complexity
```

---

# Lesson Learned

Chunking quality often matters more than people initially expect.

---

# Decision 8 - Why Conversation Memory?

# Problem

Follow-up questions lose context.

---

Example:

```text
What is self-attention?

What are its advantages?
```

---

Without memory:

```text
its = ?
```

---

# Benefits

```text
Natural Conversations

Multi-Turn Interactions

Improved UX
```

---

# Trade-Offs

```text
More Storage

Prompt Growth

Memory Management
```

---

# Lesson Learned

Users think conversationally.

Systems should support that expectation.

---

# Decision 9 - Why SSE?

# Problem

LLM responses are slow.

---

# Alternatives

```text
Traditional HTTP

Polling

WebSockets

SSE
```

---

# Why SSE Was Chosen

Benefits:

```text
Simple

HTTP-Based

Browser Friendly

Ideal For Token Streaming
```

---

# Why Not Polling?

Problems:

```text
Inefficient

Repeated Requests

Poor User Experience
```

---

# Why Not WebSockets?

While powerful:

```text
Bidirectional Communication
```

was not required.

---

# Trade-Off Accepted

```text
One-Way Communication
```

was acceptable.

---

# Lesson Learned

Streaming dramatically improves perceived performance.

---

# Decision 10 - Why Prompt Engineering?

# Problem

Retrieved context alone is not enough.

---

Need:

```text
Instructions

Grounding

Answer Rules
```

---

Benefits:

```text
Consistency

Reduced Hallucinations

Higher Quality Answers
```

---

# Lesson Learned

Prompt quality matters more than expected.

---

# Decision 11 - Why Evaluate The System?

# Problem

Answers can appear correct while being incorrect.

---

Without evaluation:

```text
Looks Good
```

becomes:

```text
Probably Good
```

---

# With Evaluation

```text
Measured Quality
```

---

Benefits:

```text
Evidence

Diagnostics

Continuous Improvement
```

---

# Lesson Learned

You cannot improve what you do not measure.

---

# Why We Did Not Start With LangChain

This is a common architect-level question.

---

# Alternative

Build using:

```text
LangChain

LlamaIndex

Framework Abstractions
```

---

# Why We Chose Fundamentals First

Goals:

```text
Understand Retrieval

Understand Prompting

Understand Streaming

Understand Memory

Understand Evaluation
```

---

Frameworks can hide implementation details.

---

# Benefits

```text
Deeper Understanding

Better Debugging

Stronger Fundamentals
```

---

# Trade-Off

```text
More Development Effort
```

---

# Lesson Learned

Learn the engine before driving the race car.

---

# Why We Did Not Start With LangGraph

# Reason

Agentic systems introduce:

```text
State Machines

Loops

Planning

Decision Making
```

---

Adding them too early increases complexity.

---

# Approach Taken

Master:

```text
Linear RAG
```

first.

---

Then evolve toward:

```text
Agentic RAG
```

---

# Lesson Learned

Complexity should be introduced gradually.

---

# Biggest Mistakes And Misconceptions

# Mistake 1

Thinking:

```text
Bigger Model
=
Better System
```

---

Reality:

```text
Retrieval Quality Often Matters More
```

---

# Mistake 2

Thinking:

```text
More Chunks
=
Better Context
```

---

Reality:

```text
More Noise
```

is often introduced.

---

# Mistake 3

Treating evaluation as an afterthought.

---

Reality:

```text
Evaluation Should Be Built Early
```

---

# Mistake 4

Underestimating observability.

---

Reality:

```text
Diagnostics Explain System Behavior
```

---

# Most Surprising Lessons Learned

# Lesson 1

Retrieval quality often matters more than model quality.

---

# Lesson 2

Prompt engineering has a larger impact than expected.

---

# Lesson 3

Streaming dramatically affects perceived performance.

---

# Lesson 4

Users care about sources and trust.

---

# Lesson 5

Diagnostics are essential for improvement.

---

# What We Would Do Differently Today

If starting again:

---

## Earlier Evaluation

Build evaluation framework sooner.

---

## Earlier Diagnostics

Improve visibility from day one.

---

## Hybrid Retrieval Preparation

Design for dense + sparse retrieval earlier.

---

## Re-Ranking Support

Plan for ranking improvements sooner.

---

## Governance Planning

Consider production controls earlier.

---

# Future Evolution Roadmap

Current State:

```text
Production-Ready RAG
```

---

Next Steps:

```text
Hybrid Search
```

↓

```text
Re-Ranking
```

↓

```text
Adaptive Retrieval
```

↓

```text
Query Rewriting
```

↓

```text
Agentic RAG
```

↓

```text
LangGraph Workflows
```

↓

```text
Multi-Agent Systems
```

↓

```text
Enterprise AI Platform
```

---

# Architect's Perspective

Good architecture balances:

```text
Technical Requirements

Business Requirements

Operational Requirements
```

---

Technical:

```text
Performance

Maintainability

Scalability
```

---

Business:

```text
Cost

Time To Market

Usability
```

---

Operational:

```text
Monitoring

Security

Reliability
```

---

The "best" solution is often the one that balances all three.

---

# Common Assessment Questions

## Why Did You Choose RAG Instead Of Fine-Tuning?

> Knowledge changes frequently, making RAG easier, cheaper, and more maintainable than constant retraining.

---

## Why Qdrant?

> Open-source, local-friendly, container-friendly, strong vector capabilities, and excellent for experimentation and learning.

---

## Why Ollama?

> Local execution, privacy, cost efficiency, and complete control over infrastructure.

---

## Why SSE Instead Of WebSockets?

> Our requirements were primarily server-to-client streaming, making SSE simpler and more appropriate.

---

## Why Clean Architecture?

> Better separation of concerns, testability, maintainability, and extensibility.

---

## Why Not LangChain First?

> Understanding underlying mechanics provides stronger foundations and improves debugging and architectural decision-making.

---

## What Was The Most Important Lesson?

> Retrieval quality and evaluation matter more than model size alone.

---

## What Would You Improve Next?

> Hybrid retrieval, re-ranking, adaptive retrieval, and agentic workflows.

---

# Key Takeaways

```text
Architecture Is About Trade-Offs.

There Is No Perfect Technology.

Requirements Drive Decisions.

Retrieval Quality Matters More Than Expected.

Prompt Engineering Is Critical.

Evaluation Is Essential.

Observability Enables Improvement.

Complexity Should Be Introduced Gradually.

Strong Fundamentals Matter More Than Framework Familiarity.

The System Was Designed Around Learning, Maintainability, And Future Evolution