# 05 - Document Preparation, Chunking And Retrieval

# Purpose of This Document

A common misconception in RAG systems is:

> "The LLM is the most important component."

In reality, many RAG failures occur **before the LLM is ever called**.

Poor answers are often caused by:

```text
Poor Document Preparation

Poor Chunking

Poor Retrieval

Poor Context Selection
```

Even the most powerful LLM cannot generate a good answer if the wrong information is retrieved.

This document explains the complete retrieval pipeline, from raw document ingestion to context retrieval.

---

# The Most Important RAG Principle

A useful mental model is:

```text
Retrieval Decides
WHAT The LLM Reads

LLM Decides
HOW The Answer Is Written
```

If retrieval fails:

```text
Wrong Context
↓
Wrong Answer
```

Therefore:

```text
Retrieval Quality
=
Answer Quality
```

---

# Overview

Document processing can be viewed as four stages:

```text
Raw Document
      ↓
Preprocessing
      ↓
Chunking
      ↓
Embeddings
      ↓
Vector Store

---------------------------------

Question
      ↓
Retrieval
      ↓
Top Relevant Chunks
      ↓
Prompt
      ↓
LLM
```

---

# Part 1 - Document Preparation

# Why Document Preparation Exists

Documents are rarely clean.

A PDF may contain:

```text
Headers

Footers

Page Numbers

Copyright Notices

Watermarks

References

Tables

Images

Empty Pages

Formatting Artifacts
```

Example:

```text
Page 12

Company Confidential

Transformer Architecture...

Page 13

Company Confidential
```

If this content is embedded directly:

```text
Company Confidential
```

appears repeatedly in embeddings.

This creates retrieval noise.

---

# Goal Of Preprocessing

Transform:

```text
Raw Document
```

into:

```text
Clean Searchable Text
```

---

# Common Preprocessing Activities

## Remove Noise

Examples:

```text
Page Numbers

Repeated Headers

Repeated Footers

Empty Lines

Watermarks
```

---

## Normalize Text

Examples:

```text
Whitespace

Encoding

Special Characters

Line Breaks
```

---

## Preserve Structure

Important information should remain.

Examples:

```text
Headings

Sections

Paragraphs

Lists
```

Document structure often provides semantic meaning.

---

# Why Preprocessing Matters

Without preprocessing:

```text
Garbage In
=
Garbage Retrieval
```

With preprocessing:

```text
Cleaner Chunks
↓
Better Embeddings
↓
Better Retrieval
```

---

# Part 2 - Chunking

# What Is Chunking?

Chunking is the process of splitting large documents into smaller units before embeddings are generated.

---

# Simple Definition

> Chunking is the process of dividing a document into manageable, retrievable pieces of information.

---

# Why Chunking Exists

Imagine a 300-page book.

Question:

```text
What is self-attention?
```

Should we retrieve:

```text
Entire Book?
```

No.

Problems:

```text
Too Large

Too Expensive

Too Much Noise

Poor Precision
```

---

Instead we retrieve:

```text
Only Relevant Sections
```

---

# Without Chunking

```text
Entire Document
      ↓
One Embedding
```

Problems:

```text
Mixed Topics

Poor Precision

Difficult Retrieval
```

---

Example:

```text
Introduction

Architecture

Training

Evaluation

Conclusion
```

all compressed into a single vector.

---

# With Chunking

```text
Document
      ↓

Chunk 1

Chunk 2

Chunk 3

Chunk 4

Chunk 5
```

Each chunk receives its own embedding.

---

Advantages:

```text
Higher Precision

Granular Retrieval

Better Context Selection
```

---

# Chunking Analogy

Imagine a textbook.

Question:

```text
What is self-attention?
```

Would a librarian give you:

```text
Entire Textbook?
```

or

```text
Relevant Chapter?
```

Chunking allows retrieval of the relevant chapter.

---

# Chunk Size

One of the most important design decisions.

---

# Extremely Small Chunks

Example:

```text
Single Sentence
```

---

Benefits:

```text
Very Precise
```

---

Problems:

```text
Context Loss

Relation Loss

Fragmentation
```

---

Example:

```text
Chunk A:

Self-attention was introduced...
```

```text
Chunk B:

...to solve long-range dependencies.
```

Meaning becomes fragmented.

---

# Extremely Large Chunks

Example:

```text
Several Pages
```

---

Benefits:

```text
More Context
```

---

Problems:

```text
More Noise

Poor Retrieval Precision

Larger Prompts
```

---

# Balanced Chunk Size

The goal is:

```text
Enough Context

Minimal Noise
```

A chunk should ideally represent a coherent idea.

Examples:

```text
Section

Topic

Paragraph Group
```

rather than:

```text
Entire Chapter

Single Line
```

---

# Chunking Strategies

# 1. Fixed-Size Chunking

Simplest approach.

Example:

```text
Every 500 Words
```

---

Advantages:

```text
Easy

Fast

Predictable
```

---

Disadvantages:

```text
May Split Ideas

Ignores Document Structure
```

---

# 2. Paragraph-Based Chunking

Uses paragraph boundaries.

---

Advantages:

```text
Natural Structure

Semantically Better
```

---

Disadvantages:

```text
Variable Chunk Sizes
```

---

# 3. Heading-Based Chunking

Uses:

```text
Section Headings

Document Structure
```

Example:

```text
## Introduction

## Attention

## Evaluation
```

---

Advantages:

```text
High Semantic Quality
```

---

Disadvantages:

```text
Dependent On Document Structure
```

---

# 4. Semantic Chunking

Most advanced approach.

---

Idea:

```text
Topic Change
      ↓
Create New Chunk
```

rather than:

```text
Every N Words
```

---

Advantages:

```text
Very Meaningful Chunks
```

---

Disadvantages:

```text
More Expensive

More Complex
```

---

# Chunk Overlap

One of the most important RAG concepts.

---

# Problem

Important information often crosses boundaries.

Example:

```text
Chunk A

The Transformer introduced
the concept of self-attention.
```

```text
Chunk B

Self-attention solved the
problem of long-range dependencies.
```

Some context is lost.

---

# Solution

Overlap.

Example:

```text
Chunk A
------------------|

Chunk B
          |------------------
```

The overlap region appears in both chunks.

---

# Benefits Of Overlap

```text
Preserves Context

Reduces Boundary Loss

Improves Recall
```

---

# Drawback Of Overlap

```text
More Chunks

More Storage

More Embeddings
```

Trade-off is usually worth it.

---

# Part 3 - Metadata Enrichment

# What Is Metadata?

Metadata is information about a chunk.

Example:

```json
{
  "Document": "attention.pdf",
  "Page": 12,
  "Section": "Self Attention",
  "ChunkId": 45
}
```

---

# Why Metadata Matters

Metadata helps with:

```text
Filtering

Source Attribution

Diagnostics

Traceability
```

---

# Examples

Instead of returning:

```text
Unknown Source
```

We can return:

```text
attention.pdf

Page 12

Section: Self Attention
```

This improves trust.

---

# Part 4 - Retrieval

# What Is Retrieval?

Retrieval is the process of finding the most relevant chunks for a question.

---

# Retrieval Workflow

Question:

```text
What is self-attention?
```

---

Step 1

Generate question embedding.

```text
Question
      ↓
Embedding
```

---

Step 2

Search Qdrant.

```text
Question Vector
        ↓
Vector Search
```

---

Step 3

Calculate similarity.

```text
Chunk A = 0.94

Chunk B = 0.89

Chunk C = 0.82
```

---

Step 4

Return best matches.

```text
Top K Chunks
```

---

# What Is Top-K Retrieval?

K means:

```text
Number Of Results Returned
```

Example:

```text
Top 1

Top 3

Top 5

Top 10
```

---

# Small K

Example:

```text
Top 1
```

Advantages:

```text
Focused

Less Noise
```

Problems:

```text
Missing Information
```

---

# Large K

Example:

```text
Top 20
```

Advantages:

```text
More Coverage
```

Problems:

```text
More Noise

Longer Prompts
```

---

# Retrieval Scores

Example:

```text
Chunk A → 0.95

Chunk B → 0.91

Chunk C → 0.84

Chunk D → 0.30
```

---

Interpretation:

```text
Higher Score
=
Higher Relevance
```

---

These scores are often based on:

```text
Cosine Similarity
```

between:

```text
Question Embedding

Chunk Embedding
```

---

# Context Assembly

The LLM never talks directly to Qdrant.

Important concept.

---

Qdrant returns:

```text
Chunk 1

Chunk 2

Chunk 3
```

---

The system creates:

```text
Context Block
```

Example:

```text
Chunk 1

----------------

Chunk 2

----------------

Chunk 3
```

---

Then:

```text
Context
+
Question
```

becomes:

```text
Prompt
```

for the LLM.

---

# Retrieval Strategies

# Dense Retrieval

What our system primarily uses.

---

Flow:

```text
Text
↓
Embedding
↓
Vector Search
```

---

Advantages:

```text
Semantic Understanding
```

---

# Sparse Retrieval

Traditional approach.

Examples:

```text
BM25

TF-IDF

Keyword Search
```

---

Advantages:

```text
Exact Keyword Matching
```

---

Weakness:

```text
Limited Semantic Understanding
```

---

# Hybrid Retrieval

Combines:

```text
Dense Search

+
Sparse Search
```

---

Purpose:

```text
Best Of Both Worlds
```

---

Many production systems use:

```text
Hybrid Retrieval
```

for improved accuracy.

---

# Re-Ranking

Advanced retrieval concept.

---

Initial Retrieval:

```text
Top 20 Chunks
```

---

Re-Ranker:

```text
Evaluates Relevance Again
```

---

Output:

```text
Best Top 5 Chunks
```

---

Benefits:

```text
Higher Answer Quality

Better Relevance
```

---

# Retrieval Quality

# How Do We Know Retrieval Is Good?

A common assessment question.

---

Good retrieval should provide:

```text
Relevant Information

Complete Information

Trustworthy Information
```

---

Important factors:

```text
Precision

Recall

Coverage

Grounding
```

---

# Precision

Question:

```text
Of retrieved chunks,
how many were actually relevant?
```

Higher precision:

```text
Less Noise
```

---

# Recall

Question:

```text
Did we retrieve all important information?
```

Higher recall:

```text
Less Missing Context
```

---

# Coverage

Question:

```text
Was enough supporting information retrieved?
```

---

# Part 5 - Common Failure Scenarios

# Chunk Too Small

```text
Context Lost
```

↓

```text
Poor Answers
```

---

# Chunk Too Large

```text
Too Much Noise
```

↓

```text
Poor Retrieval Precision
```

---

# No Overlap

```text
Boundary Information Lost
```

↓

```text
Incomplete Answers
```

---

# Poor Document Cleaning

```text
Noisy Chunks
```

↓

```text
Noisy Retrieval
```

---

# Wrong Top-K

Too Small:

```text
Missing Information
```

Too Large:

```text
Too Much Noise
```

---

# Good LLM + Bad Retrieval

One of the most important lessons.

```text
Excellent Model

+

Wrong Context

=

Wrong Answer
```

---

# Relationship To Hallucination

Without retrieval:

```text
Question
↓
LLM Guess
↓
Possible Hallucination
```

---

With retrieval:

```text
Question
↓
Evidence Retrieved
↓
LLM Generates
↓
Grounded Answer
```

---

Good retrieval significantly reduces hallucinations.

---

# Our Implementation

Our solution includes:

```text
PDF Text Extraction

Document Preparation

Chunk Generation

Embedding Generation

Qdrant Storage

Vector Search

Top-K Retrieval

Source Attribution

Retrieval Diagnostics
```

---

# Retrieval Diagnostics We Exposed

Examples:

```text
Retrieval Time

Returned Chunks

Average Score

Highest Score

Lowest Score

Sources
```

These provide visibility into retrieval behavior.

---

# Common Assessment Questions

## Why Do We Need Chunking?

> Large documents are too broad and inefficient to retrieve. Chunking improves retrieval granularity and precision.

---

## Why Not Embed Entire Documents?

> Entire documents often contain multiple topics. A single embedding becomes too broad and reduces retrieval effectiveness.

---

## What Is Chunk Overlap?

> Overlap duplicates a small portion of adjacent chunks to preserve context across chunk boundaries.

---

## What Is Top-K Retrieval?

> Top-K retrieval returns the K most relevant chunks based on similarity scores.

---

## Why Can Chunk Size Affect Answer Quality?

> Small chunks may lose context, while large chunks introduce noise. Both can negatively impact retrieval quality.

---

## What Is Metadata Used For?

> Metadata supports filtering, traceability, diagnostics, and source attribution.

---

## What Is Hybrid Search?

> Hybrid retrieval combines semantic vector search with traditional keyword search.

---

## What Is Re-Ranking?

> Re-ranking reevaluates retrieved chunks and promotes the most relevant context before prompt construction.

---

## How Does Retrieval Reduce Hallucination?

> Retrieval provides evidence-based context so the model can answer using retrieved information instead of guessing.

---

# Key Takeaways

```text
Document Preparation Improves Data Quality.

Chunking Creates Retrievable Units.

Chunk Size Is A Trade-Off.

Overlap Preserves Context.

Metadata Improves Traceability.

Retrieval Finds Relevant Chunks.

Top-K Controls Context Volume.

Hybrid Retrieval Combines Multiple Approaches.

Re-Ranking Improves Relevance.

Good Retrieval Often Matters More Than Model Size.
```

---

# 60-Second Assessment Answer

> "Chunking is the process of splitting large documents into smaller, meaningful units before generating embeddings. Prior to chunking, documents are typically cleaned and normalized to remove noise such as headers, footers, and formatting artifacts. Each chunk is embedded and stored in a vector database along with metadata. During question answering, the user's question is converted into an embedding and used to perform similarity search against stored chunk embeddings. The most relevant chunks are selected through Top-K retrieval and assembled into the prompt. Techniques such as overlap, metadata enrichment, hybrid retrieval, and re-ranking can further improve retrieval quality. Effective chunking and retrieval are critical because the quality of retrieved context directly influences answer quality and grounding."