# 08 - Streaming And Server-Sent Events (SSE)

# Purpose of This Document

Traditional APIs use a request-response model:

```text
Request
   ↓
Server Processes
   ↓
Response Returned
```

This works well for:

```text
CRUD APIs

Configuration APIs

Metadata Queries

Simple Business Operations
```

However, AI applications often take several seconds to generate responses.

If users must wait for the entire answer to finish before seeing anything, the experience feels slow and unresponsive.

Modern AI systems solve this problem through:

```text
Streaming
```

This document explains:

- What streaming is
- Why streaming is important
- What Server-Sent Events (SSE) are
- Why SSE is a great fit for AI applications
- How our implementation works
- SSE versus WebSockets
- Streaming challenges
- Future improvements
- Assessment-oriented explanations

---

# The Problem

Imagine the user asks:

```text
Explain the Transformer architecture in detail.
```

The LLM requires:

```text
15 – 30 seconds
```

to generate the full answer.

---

# Traditional API Experience

```text
User
  ↓
Send Request
  ↓

(wait...)

(wait...)

(wait...)

  ↓
Entire Response Arrives
```

From the user's perspective:

```text
Nothing seems to be happening.
```

This creates uncertainty:

```text
Is the system working?

Did the request fail?

Is the server slow?

Should I refresh?
```

---

# Streaming Experience

Instead of waiting for the entire answer:

```text
Transformer

Transformer architecture

Transformer architecture uses

Transformer architecture uses self-attention

...
```

The user immediately sees progress.

---

# Why Streaming Matters

Streaming provides:

```text
Better User Experience

Improved Responsiveness

Perceived Faster Performance

Live Feedback

ChatGPT-Like Interaction
```

---

# Human Conversation Analogy

Imagine asking a colleague:

```text
Explain microservices architecture.
```

---

Without streaming:

```text
Question
 ↓

30 Seconds Silence

 ↓

Complete Answer
```

Odd and uncomfortable.

---

With streaming:

```text
Question
 ↓

"Well..."

"Microservices..."

"Microservices are..."

...
```

Feels much more natural.

---

# What Is Streaming?

Streaming means:

```text
Sending Data Incrementally
```

instead of:

```text
Sending Everything At The End
```

---

# Traditional Response

```text
Generate Entire Answer
        ↓
Send Answer
```

---

# Streaming Response

```text
Generate Some Content
        ↓
Send Content

Generate More Content
        ↓
Send More Content

Generate More Content
        ↓
Send More Content
```

The client continuously receives updates.

---

# AI Token Generation

LLMs naturally generate responses one token at a time.

Example:

```text
Token 1:
Self

Token 2:
-attention

Token 3:
is

Token 4:
a

Token 5:
mechanism
```

The answer does not suddenly appear all at once.

The model generates it incrementally.

Streaming allows users to see those tokens as they are produced.

---

# What Is SSE?

SSE stands for:

```text
Server-Sent Events
```

---

# Definition

> Server-Sent Events is an HTTP-based mechanism that allows a server to continuously send data to a client over a long-lived connection.

---

# Core Idea

Instead of:

```text
Open Connection
 ↓
Receive Response
 ↓
Close Connection
```

SSE does:

```text
Open Connection
 ↓

Send Event

Send Event

Send Event

Send Event

 ↓

Close Connection
```

---

# Why SSE Works Well For AI Chat

AI answer generation is primarily:

```text
Server
 ↓
Client
```

communication.

The browser mainly consumes information.

There is very little need for continuous bidirectional communication.

---

Therefore:

```text
SSE
```

is usually a perfect fit.

---

# SSE Mental Model

Think of SSE as:

```text
A Live Feed
```

The server keeps publishing updates.

The browser keeps listening.

---

# SSE Event Format

Events are text-based.

Example:

```text
event: token
data: "Transformer"
```

---

Another event:

```text
event: token
data: "architecture"
```

---

Another event:

```text
event: token
data: "uses"
```

---

Final event:

```text
event: completed
data: {...}
```

---

# Generic Structure

```text
event: EventName

data: EventPayload
```

---

# SSE Lifecycle

```text
Open Connection
 ↓

Receive Event

Receive Event

Receive Event

Receive Event

 ↓

Completed Event

 ↓

Close Connection
```

---

# Event Types In Our Solution

Our implementation streams different event categories.

---

# Retrieval Event

Purpose:

```text
Expose Retrieval Information
```

Contains:

```text
Retrieved Sources

Similarity Scores

Chunk Information

Retrieval Metadata
```

---

Example:

```text
event: retrieval
```

---

# Token Event

Purpose:

```text
Stream Generated Content
```

Contains:

```text
Generated Tokens
```

---

Example:

```text
event: token
```

---

Token sequence:

```text
Self

Self-attention

Self-attention is

Self-attention is a
```

---

# Completed Event

Purpose:

```text
Signal Generation Completion
```

Contains:

```text
Generation Metrics

Diagnostics

Completion Metadata
```

---

Example:

```text
event: completed
```

---

# Error Event

Purpose:

```text
Communicate Failures
```

Examples:

```text
LLM Failure

Retrieval Failure

Network Issue

Unexpected Exception
```

---

# End-To-End Streaming Flow

```text
User
 │
 │ Question
 ▼

API
 │
 │ Retrieve Context
 ▼

Qdrant
 │
 ▼

Prompt Builder
 │
 ▼

Ollama
 │
 │ Token Stream
 ▼

SSE Endpoint
 │
 ▼

Browser
 │
 ▼

Real-Time Rendering
```

---

# How Streaming Works In Our Solution

Step 1:

```text
User submits question
```

---

Step 2:

```text
Conversation endpoint receives request
```

---

Step 3:

```text
Document retrieval executes
```

---

Step 4:

```text
Prompt is constructed
```

using:

```text
System Prompt

Conversation History

Retrieved Context

Current Question
```

---

Step 5:

```text
Prompt sent to Ollama
```

---

Step 6:

```text
Ollama begins token generation
```

Example:

```text
Self

Self-attention

Self-attention is

Self-attention is a
```

---

Step 7:

Each token is published as:

```text
event: token
```

---

Step 8:

Browser receives tokens immediately.

---

Step 9:

UI updates message incrementally.

---

Step 10:

Completed event is emitted.

---

# Browser Side Processing

The browser continuously listens for events.

Flow:

```text
Receive Event
      ↓

Parse Event
      ↓

Determine Event Type
      ↓

Update UI
      ↓

Auto Scroll
```

---

# Example

Token arrives:

```text
Self
```

UI becomes:

```text
Self
```

---

Next token:

```text
-attention
```

UI becomes:

```text
Self-attention
```

---

Next token:

```text
is
```

UI becomes:

```text
Self-attention is
```

---

This continues until completion.

---

# Why Streaming Feels Faster

Important assessment concept.

Streaming does NOT necessarily reduce:

```text
Actual Processing Time
```

---

Example:

```text
Without Streaming:
20 Seconds Total
```

---

```text
With Streaming:
20 Seconds Total
```

---

The difference is:

```text
Users See Progress
```

instead of waiting in silence.

---

This improves:

```text
Perceived Performance
```

---

# Traditional API Versus Streaming

## Traditional API

```text
Request
 ↓

Wait

Wait

Wait

 ↓

Response
```

---

Advantages:

```text
Simple

Easy To Implement
```

---

Disadvantages:

```text
Poor Long-Running Experience
```

---

# Streaming API

```text
Request
 ↓

Partial Response

Partial Response

Partial Response

Completed
```

---

Advantages:

```text
Interactive Experience

Live Feedback

User Engagement
```

---

Disadvantages:

```text
More Complex
```

---

# SSE Versus Polling

Before SSE, many systems used polling.

---

# Polling Workflow

```text
Browser:
Any Updates?

Server:
No

Browser:
Any Updates?

Server:
No

Browser:
Any Updates?

Server:
No
```

Repeated continuously.

---

Problems:

```text
Many Requests

Wasted Resources

Higher Latency
```

---

SSE is more efficient.

---

# SSE Versus WebSockets

This is one of the most common assessment questions.

---

# SSE

Communication:

```text
Server
 ↓
Client
```

One-way.

---

# WebSockets

Communication:

```text
Client
 ↕
Server
```

Two-way.

---

# SSE Advantages

```text
Simple

HTTP-Based

Browser Friendly

Ideal For AI Responses

Easy To Debug
```

---

# WebSocket Advantages

```text
Bidirectional Communication

Online Gaming

Live Collaboration

Realtime Applications
```

---

# Why We Selected SSE

Our use case:

```text
Generate Answer
 ↓
Send To Browser
```

We primarily need:

```text
Server → Client
```

communication.

---

SSE provides:

```text
Less Complexity

Simple Architecture

Excellent Browser Support

Easy Integration
```

---

# Why Not WebSockets?

We do not require:

```text
Continuous Browser Messages

Realtime Multiplayer Communication

Collaborative Editing
```

Therefore WebSockets would add complexity without significant benefits.

---

# Streaming Challenges

Real systems encounter many challenges.

---

# Network Interruptions

Connection may terminate unexpectedly.

Result:

```text
Partial Response
```

---

# Browser Refresh

User refreshes page.

Result:

```text
Connection Closed
```

---

# Model Failure

The LLM may fail:

```text
Timeout

Crash

Unexpected Error
```

---

The client must handle:

```text
error events
```

gracefully.

---

# Partial Responses

Generation may stop before completion.

The UI should distinguish between:

```text
Completed Answer
```

and

```text
Incomplete Answer
```

---

# User Experience Challenges

Streaming introduces frontend concerns.

Examples:

```text
Typing Indicator

Streaming Cursor

Auto Scroll

Rendering Performance

Message Completion
```

---

# Real Challenges We Encountered

During implementation we worked through:

```text
Streaming Cursor Rendering

Typing Indicator Cleanup

Bubble Width Improvements

Incremental Token Rendering

SSE Event Handling

Conversation Reset Support
```

These are common real-world streaming challenges.

---

# Streaming And Observability

One useful enhancement in our implementation was:

```text
Streaming Answer

+

Retrieval Diagnostics

+

Generation Metrics
```

---

Users could see:

```text
Sources

Retrieval Details

Performance Information
```

alongside answers.

---

# Streaming And Clean Architecture

Notice something important.

Adding streaming required changes mainly in:

```text
API Layer

UI Layer
```

---

It did NOT require major changes to:

```text
Chunking

Retrieval

Embeddings

Prompt Engineering
```

This demonstrates good separation of concerns.

---

# Future Enhancements

## Stop Generation

Add:

```text
Stop Button
```

to cancel long-running responses.

---

## Reconnect Support

Allow:

```text
Connection Recovery
```

after interruption.

---

## Advanced Markdown Rendering

Render:

```text
Lists

Tables

Code Blocks
```

incrementally.

---

## Multi-Modal Streaming

Future responses may stream:

```text
Text

Images

Tool Calls

Reasoning Steps
```

---

# Common Assessment Questions

## Why Is Streaming Important?

> Streaming improves user experience by displaying content as it is generated rather than waiting for the complete response.

---

## What Is SSE?

> Server-Sent Events is an HTTP-based mechanism that allows servers to continuously push events to clients over a long-lived connection.

---

## How Is SSE Different From Traditional APIs?

> Traditional APIs return a single response, whereas SSE returns multiple events over time.

---

## Why Was SSE Chosen Instead Of WebSockets?

> Our use case primarily requires one-way communication from server to browser, making SSE simpler and more appropriate.

---

## Does Streaming Reduce Actual Generation Time?

> No. Streaming mainly improves perceived performance by showing progress immediately.

---

## What Types Of Events Did We Stream?

> Retrieval events, token events, completion events, and error events.

---

## How Does The Browser Render The Response?

> Each streamed token is received, parsed, appended to the current message, and rendered incrementally in the chat window.

---

## What Happens If The Connection Drops?

> The stream may end prematurely, and the UI should handle incomplete responses gracefully.

---

# Key Takeaways

```text
Streaming Improves User Experience.

LLMs Naturally Generate Tokens Incrementally.

SSE Streams Events Over A Long-Lived HTTP Connection.

SSE Is Well Suited For AI Chat Applications.

Streaming Improves Perceived Performance.

Token Events Create ChatGPT-Like Experiences.

SSE Is Simpler Than WebSockets For One-Way Communication.

Real-Time Rendering Requires Careful UI Handling.

Streaming Integrated Cleanly With Our Architecture.
```

---

# 60-Second Assessment Answer

> "Streaming allows generated content to be sent incrementally instead of waiting for the entire response to complete. Our solution uses Server-Sent Events (SSE), which maintains a long-lived HTTP connection and continuously sends events such as retrieval metadata, generated tokens, completion events, and error notifications. As Ollama generates tokens, the API forwards them as SSE events, and the browser updates the chat interface in real time. We