# 10 - Advanced RAG Patterns And Agentic Workflows

# Purpose of This Document

The first generation of RAG systems followed a very simple architecture:

```text
Question
 ↓
Retrieve
 ↓
Generate
 ↓
Answer
```

This architecture works well for straightforward questions such as:

```text
What is self-attention?

What is an embedding?

What is a vector database?
```

However, real-world problems are rarely that simple.

Users ask:

```text
Compare architectures

Analyze multiple documents

Perform investigations

Identify trends

Summarize complex topics

Validate findings
```

Such questions often require:

```text
Multiple Retrieval Steps

Dynamic Decision Making

Query Transformation

Iterative Reasoning

Context Validation
```

This gives rise to:

```text
Advanced RAG

Agentic RAG

Workflow-Based AI Systems

LangGraph Style Pipelines
```

This document serves as a foundational guide to advanced RAG concepts and future evolution paths for our solution.

---

# Evolution Of RAG Systems

# Generation 1 - Naive RAG

Traditional workflow:

```text
Question
 ↓
Embedding
 ↓
Vector Search
 ↓
Prompt
 ↓
LLM
 ↓
Answer
```

---

Advantages:

```text
Simple

Fast

Easy To Implement
```

---

Limitations:

```text
Single Retrieval

Fixed Workflow

No Planning

No Self-Correction
```

---

# Generation 2 - Advanced RAG

Workflow:

```text
Question
 ↓
Rewrite Query
 ↓
Retrieve
 ↓
Re-rank
 ↓
Generate
 ↓
Answer
```

---

Capabilities:

```text
Better Retrieval

Higher Recall

Better Precision

Improved Context Quality
```

---

# Generation 3 - Agentic RAG

Workflow:

```text
Question
 ↓
Plan
 ↓
Retrieve
 ↓
Evaluate
 ↓

Enough Context?
     │
 ┌───┴─────┐
 │         │
 No       Yes
 │         │
 ▼         ▼

Retrieve  Answer
 Again
```

---

Capabilities:

```text
Reasoning

Planning

Decision Making

Adaptive Retrieval

Self-Correction
```

---

# The Core Limitation Of Traditional RAG

Traditional RAG assumes:

```text
One Search
=
Enough Information
```

This assumption often breaks down.

---

Example:

```text
Compare RNNs and Transformers.
```

To answer properly we need:

```text
Information About RNNs

Information About Transformers

Advantages

Disadvantages

Performance Characteristics
```

One retrieval may fail to retrieve all relevant context.

---

Another example:

```text
Summarize all security concerns
mentioned across uploaded documents.
```

A single search result is unlikely to be sufficient.

---

This is where advanced retrieval workflows become valuable.

---

# Query Rewriting

# What Is Query Rewriting?

Query rewriting transforms a user question into a retrieval-friendly query.

---

# Problem Example

User asks:

```text
What are its advantages?
```

Retriever sees:

```text
its
```

which provides almost no meaning.

---

# Solution

Rewrite the question.

Example:

```text
What are the advantages of self-attention?
```

---

Pipeline:

```text
Question
 ↓
Rewrite
 ↓
Retrieve
 ↓
Answer
```

---

# Why Query Rewriting Works

Many user questions are:

```text
Ambiguous

Incomplete

Conversational

Poor Search Queries
```

---

Rewriting improves:

```text
Precision

Recall

Retrieval Quality
```

---

# Query Rewriting Types

---

## Context-Aware Rewriting

Original:

```text
What are its advantages?
```

---

Rewritten:

```text
What are the advantages of self-attention?
```

---

Uses conversation memory.

---

## Clarification Rewriting

Original:

```text
Explain transformers.
```

---

Rewritten:

```text
Explain the Transformer neural network architecture.
```

---

Improves retrieval specificity.

---

# Query Expansion

# What Is Query Expansion?

Query expansion broadens retrieval coverage.

---

Original query:

```text
Embeddings
```

Expanded query:

```text
Embeddings

Vector Representations

Semantic Vectors

Embedding Models
```

---

Purpose:

```text
Increase Recall
```

---

# Why Query Expansion Helps

Different documents may use different terminology.

Example:

```text
Embedding
```

vs

```text
Vector Representation
```

---

A single query may miss relevant chunks.

Expanded queries improve retrieval coverage.

---

# Multi-Query Retrieval

One of the most powerful retrieval techniques.

---

# Traditional Retrieval

```text
Question
 ↓
Single Search
 ↓
Results
```

---

# Multi-Query Retrieval

```text
Question
 ↓

Generate Multiple Queries
 ↓

Retrieve Independently
 ↓

Merge Results
```

---

# Example

Question:

```text
Compare RNNs and Transformers.
```

---

Generated subqueries:

```text
What are RNNs?

What are Transformers?

Advantages of RNNs

Advantages of Transformers

Differences Between RNNs And Transformers
```

---

Results are merged into a richer context.

---

# Benefits

```text
Better Coverage

Higher Recall

Improved Comparison Answers
```

---

# Adaptive Retrieval

Traditional systems often use:

```text
Top K = 5
```

for every question.

---

This may not always be optimal.

---

# Simple Question

```text
What is self-attention?
```

Required:

```text
3 Chunks
```

may be sufficient.

---

# Complex Question

```text
Compare Transformers, RNNs,
and CNNs.
```

May require:

```text
10-15 Chunks
```

---

Dynamic retrieval adjusts retrieval strategy based on question complexity.

---

# Dynamic Top-K

Instead of:

```text
Always 5
```

Use:

```text
Simple Questions → Top 3

Moderate Questions → Top 5

Complex Questions → Top 10
```

---

Advantages:

```text
Reduce Noise

Improve Coverage

Lower Costs
```

---

# Adaptive Retrieval Flow

```text
Question
 ↓

Complexity Assessment
 ↓

Determine Retrieval Strategy
 ↓

Retrieve
 ↓

Generate
```

---

# Hybrid Retrieval

One retrieval strategy rarely captures everything.

---

# Dense Retrieval

Uses:

```text
Embeddings

Vector Similarity
```

---

Strengths:

```text
Semantic Understanding
```

---

# Sparse Retrieval

Uses:

```text
Keywords

BM25

TF-IDF
```

---

Strengths:

```text
Exact Term Matching
```

---

# Hybrid Retrieval

Combines both.

```text
Dense Search

+

Sparse Search
```

---

Benefits:

```text
Best Of Both Approaches
```

---

Widely used in enterprise RAG systems.

---

# Re-Ranking

# The Problem

Initial retrieval may produce:

```text
Top 20 Results
```

---

Not all are equally useful.

---

# Re-Ranking Flow

```text
Question
 ↓

Retrieve Top 20
 ↓

Re-Ranker
 ↓

Best Top 5
```

---

# Re-Ranker Purpose

Perform deeper relevance analysis.

---

Initial similarity may produce:

```text
Chunk A → Rank 1

Chunk B → Rank 2

Chunk C → Rank 3
```

---

Re-ranker may reorder:

```text
Chunk C → Rank 1

Chunk A → Rank 2

Chunk B → Rank 3
```

based on contextual relevance.

---

# Benefits

```text
Higher Precision

Reduced Noise

Better Prompt Context
```

---

# Context Compression

Sometimes retrieval returns:

```text
Too Much Context
```

---

Context compression creates:

```text
Smaller

Relevant

Focused
```

context blocks.

---

Useful for:

```text
Token Efficiency

Latency Reduction

Prompt Optimization
```

---

# Self-Correcting Retrieval

# Traditional Model

```text
Retrieve Once
 ↓
Answer
```

---

# Self-Correcting Model

```text
Retrieve
 ↓

Evaluate Retrieval
 ↓

Enough Information?
```

---

If No:

```text
Retrieve Again
```

---

Benefits:

```text
Improved Reliability

Better Coverage

Reduced Missing Context
```

---

# Reflection Pattern

One of the most interesting emerging patterns.

---

# Concept

The model reviews its own output.

---

Workflow:

```text
Generate Draft
 ↓

Critique Draft
 ↓

Improve Draft
 ↓

Return Answer
```

---

This introduces:

```text
Self-Evaluation

Self-Correction

Quality Improvement
```

---

# Example

Draft:

```text
Transformer introduced attention.
```

---

Reflection:

```text
Incomplete answer.

Need to mention self-attention.
```

---

Improved answer:

```text
Transformer introduced the self-attention
mechanism and removed recurrence.
```

---

# Multi-Hop Reasoning

Many real-world questions require multiple reasoning steps.

---

Question:

```text
Which architecture introduced
self-attention and what limitation
was it designed to address?
```

---

Required process:

Step 1:

```text
Find architecture
```

---

Step 2:

```text
Find limitation
```

---

Step 3:

```text
Connect both findings
```

---

This process is known as:

```text
Multi-Hop Reasoning
```

---

# Agentic RAG

# What Is Agentic RAG?

Agentic RAG extends traditional RAG by allowing the system to make decisions about its workflow.

---

Traditional RAG:

```text
Fixed Pipeline
```

---

Agentic RAG:

```text
Dynamic Pipeline
```

---

The system can ask:

```text
Do I have enough information?

Should I search again?

Should I query another source?

Should I verify the answer?
```

---

# Definition

> Agentic RAG combines retrieval, reasoning, planning, and decision-making to dynamically determine how information should be collected and used.

---

# Traditional RAG vs Agentic RAG

Traditional:

```text
Retrieve
 ↓
Generate
```

---

Agentic:

```text
Plan
 ↓
Retrieve
 ↓
Evaluate
 ↓
Reason
 ↓
Retrieve Again
 ↓
Generate
```

---

# What Is An Agent?

An agent is a system that can:

```text
Observe

Reason

Decide

Act
```

to achieve a goal.

---

Example Goal:

```text
Answer User Question
```

---

Possible actions:

```text
Retrieve Documents

Search Knowledge

Rewrite Query

Generate Answer

Evaluate Result
```

---

# Agent Loop Mental Model

```text
Goal
 ↓

Think
 ↓

Act
 ↓

Observe
 ↓

Think Again
 ↓

Complete
```

---

# Planner Pattern

Many agents begin with planning.

---

Question:

```text
Compare RNNs and Transformers.
```

---

Planner creates:

```text
Retrieve RNN Information

Retrieve Transformer Information

Generate Comparison
```

---

Execution becomes structured.

---

# Tool-Using Agents

Modern agents may interact with tools.

---

Examples:

```text
Vector Search

Databases

Search Engines

APIs

Calculators

Files
```

---

Workflow:

```text
Question
 ↓

Choose Tool
 ↓

Execute Tool
 ↓

Analyze Result
 ↓

Respond
```

---

# LangGraph Concepts

LangGraph is built around stateful workflows.

---

Think of LangGraph as:

```text
State Machine

+

Workflow Engine

+

AI Components
```

---

# Why LangGraph Exists

Simple chains work for:

```text
Linear Pipelines
```

---

Complex workflows require:

```text
Branches

Loops

Conditional Logic

Retries

State
```

---

LangGraph addresses these needs.

---

# Core LangGraph Concepts

---

# State

State is information shared across workflow execution.

---

Example:

```text
Question

Retrieved Chunks

Reasoning Results

Intermediate Outputs

Final Answer
```

---

State moves through the workflow.

---

# Nodes

Nodes are workflow steps.

Examples:

```text
Rewrite Query

Retrieve

Generate

Evaluate

Summarize
```

---

Each node performs a specific task.

---

# Edges

Edges connect nodes.

Example:

```text
Retrieve
 ↓
Generate
```

---

Edges define workflow flow.

---

# Conditional Routing

One of the most powerful concepts.

---

Question:

```text
Enough Context?
```

---

If Yes:

```text
Generate Answer
```

---

If No:

```text
Retrieve Again
```

---

The workflow becomes adaptive.

---

# Loops

Traditional pipelines are linear.

---

LangGraph supports:

```text
Retrieve
 ↓

Evaluate
 ↓

Need More Context?
 ↓

Retrieve Again
```

---

Looping enables self-correction.

---

# LangGraph Mental Model

```text
Nodes
+
State
+
Decision Logic
+
Loops
```

creates:

```text
Agent Workflows
```

---

# State Machines And RAG

Viewing Agentic RAG as a state machine is extremely helpful.

---

Example states:

```text
QuestionReceived

QueryRewritten

Retrieving

Evaluating

Generating

Completed
```

---

Transitions occur based on evaluation outcomes.

---

This mental model makes large AI workflows easier to understand.

---

# Future Evolution Of Our Solution

Current architecture:

```text
Question
 ↓
Retrieve
 ↓
Prompt
 ↓
Generate
 ↓
Answer
```

---

Near-term enhancements:

```text
Query Rewriting

Hybrid Retrieval

Re-Ranking

Adaptive K

Context Compression
```

---

Medium-term enhancements:

```text
Multi-Query Retrieval

Retrieval Validation

Reflection

Self-Correcting Retrieval
```

---

Long-term vision:

```text
Planner

Agents

State Machines

LangGraph Workflows

Tool Usage

Adaptive Reasoning
```

---

# Benefits Of Agentic Systems

```text
Higher Quality Answers

Better Coverage

Improved Retrieval

Dynamic Adaptation

Reduced Missing Context

Improved Reasoning
```

---

# Challenges Of Agentic Systems

```text
Higher Cost

Longer Latency

More Complexity

Debugging Difficulty

Observability Challenges

Workflow Management
```

---

# What We Built vs Future State

## Current Solution

```text
Vector Search

Conversation Memory

Prompt Engineering

Streaming

Grounded Answers

Evaluation

Diagnostics
```

---

## Future Evolution

```text
Query Rewriting

Multi-Query Retrieval

Adaptive Retrieval

Hybrid Search

Re-Ranking

Reflection

Agentic RAG

LangGraph Workflows
```

---

# Common Assessment Questions

## What Is Agentic RAG?

> Agentic RAG extends traditional RAG by allowing a system to plan, reason, evaluate, and dynamically decide how retrieval and generation should occur.

---

## What Is Query Rewriting?

> Query rewriting transforms user questions into retrieval-optimized queries to improve search quality.

---

## What Is Multi-Query Retrieval?

> A technique that generates multiple search queries from a single question and merges retrieved results.

---

## What Is Dynamic Top-K?

> Dynamic Top-K adjusts the number of retrieved chunks based on question complexity instead of using a fixed value.

---

## Why Is Re-Ranking Useful?

> Re-ranking improves retrieval quality by reordering retrieved chunks based on deeper relevance analysis.

---

## What Is Reflection?

> Reflection is a pattern where an LLM reviews and improves its own generated output before returning a final response.

---

## What Is Hybrid Retrieval?

> Hybrid retrieval combines dense semantic search and sparse keyword search to improve overall retrieval quality.

---

## What Is LangGraph?

> LangGraph is a framework for building stateful AI workflows using nodes, edges, state, loops, and conditional routing.

---

## Why Are Loops Useful In AI Workflows?

> Loops allow systems to self-correct, retrieve additional information, and refine answers before finalizing a response.

---

# Key Takeaways

```text
Traditional RAG Uses Fixed Workflows.

Advanced RAG Improves Retrieval Quality.

Query Rewriting Increases Retrieval Accuracy.

Multi-Query Retrieval Improves Recall.

Adaptive Retrieval Adjusts To Question Complexity.

Hybrid Search Combines Semantic And Keyword Retrieval.

Re-Ranking Improves Context Selection.

Reflection Enables Self-Correction.

Agentic RAG Introduces Planning And Decision-Making.

LangGraph Enables Stateful, Dynamic AI Workflows.

The Future Of Enterprise RAG Is Increasingly Agentic.
```

---

# 60-Second Assessment Answer

> "Traditional RAG uses a fixed pipeline where a question is embedded, relevant chunks are retrieved, and an answer is generated. While this works well for many use cases, complex questions often require more sophisticated workflows. Advanced RAG introduces techniques such as query rewriting, query expansion, multi-query retrieval, hybrid search, adaptive Top-K selection, re-ranking, and context compression to improve retrieval quality. Agentic RAG extends this further by allowing systems to plan, reason, evaluate results, and dynamically decide what actions to perform next. Frameworks such as LangGraph model these workflows using state, nodes, edges, conditional routing, and loops, enabling AI systems to self-correct, perform multi-step reasoning, and adapt their workflow instead of following a fixed sequence of steps."