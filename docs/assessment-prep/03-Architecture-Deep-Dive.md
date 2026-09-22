# 03 - Architecture Deep Dive

# Purpose of This Document

One of the strongest aspects of our RAG solution was its architecture.

While many RAG demonstrations focus only on:

```text
PDF
↓
Embeddings
↓
Vector Search
↓
LLM
```

our implementation intentionally adopted **Clean Architecture** principles to ensure:

- Maintainability
- Testability
- Extensibility
- Technology independence
- Separation of concerns

This document explains the architectural decisions behind the solution and provides the reasoning needed for assessments, interviews, and architecture discussions.

---

# Why Architecture Matters

The goal of architecture is not simply to organize files.

Good architecture determines:

```text
How easy the system is to modify.

How easy the system is to test.

How easy the system is to extend.

How resilient the solution is to change.
```

---

# The Reality Of Software Projects

Most systems start small:

```text
Single Project
Single Service
Few Classes
```

Then requirements grow:

```text
Add Evaluation
Add Memory
Add Diagnostics
Add UI
Add Authentication
Add New Models
```

Without architectural boundaries:

```text
Everything becomes tightly coupled.
```

This leads to:

```text
Difficult testing

Difficult maintenance

Fear of change

Growing technical debt
```

---

# Why We Chose Clean Architecture

Our goal was to build a RAG solution that could evolve over time.

We wanted to support:

```text
Replace Ollama later

Replace Qdrant later

Add Azure OpenAI later

Add PostgreSQL later

Add New UIs later

Add New Evaluation Strategies later
```

without rewriting the entire system.

---

# What Is Clean Architecture?

Clean Architecture is a design approach where:

```text
Business Rules
        ↓
remain independent of
        ↓
Frameworks & External Systems
```

The most important principle is:

> Dependencies always point inward.

---

# Clean Architecture Mental Model

Instead of:

```text
Application
    ↓
Directly Depends On
    ↓
Database
    ↓
External APIs
```

we organize the system into layers.

```text
API
 ↓

Application
 ↓

Domain

Infrastructure
```

The inner layers do not know about the outer layers.

---

# Core Principle

```text
Business Logic
       ↑
Should Not Depend On
       ↓
Technology Choices
```

Example:

Business logic should not know:

```text
Qdrant

Ollama

ASP.NET

HTTP

JSON

SSE
```

These are implementation details.

---

# Our Solution Structure

```text
RagDemo.Api

RagDemo.Application

RagDemo.Domain

RagDemo.Infrastructure
```

Each project has a specific responsibility.

---

# Layer Overview

```text
+-------------------+
| API               |
+-------------------+

+-------------------+
| Application       |
+-------------------+

+-------------------+
| Domain            |
+-------------------+

+-------------------+
| Infrastructure    |
+-------------------+
```

---

# Domain Layer

# Purpose

The Domain layer contains:

```text
Core Business Concepts
Rules
Contracts
Abstractions
```

It represents:

```text
What the system does
```

not

```text
How the system does it
```

---

# What Lives In Domain?

Examples:

```text
Entities

Value Objects

Interfaces

Contracts

Core Models
```

---

# Examples From Our Solution

Examples include:

```text
Document

Chunk

Embedding

RetrievedContext

Conversation

EvaluationResult
```

These concepts exist regardless of technology choices.

---

# What Must NOT Exist In Domain?

No:

```text
ASP.NET Core

Qdrant SDK

Ollama SDK

PDF Libraries

HTTP Endpoints
```

Why?

Because the domain must remain technology-independent.

---

# Domain Layer Analogy

Imagine:

```text
Banking System
```

The concept:

```text
Bank Account
```

exists regardless of:

```text
Database

Web Framework

Cloud Provider
```

Similarly:

```text
Conversation

Document

Chunk

Embedding
```

exist regardless of Qdrant or Ollama.

---

# Application Layer

# Purpose

The Application layer contains:

```text
Use Cases

Business Workflows

Orchestration Logic
```

It coordinates the system.

---

# Key Question

The Domain answers:

```text
What?
```

The Application answers:

```text
How?
```

---

# Example Use Cases

Examples:

```text
Ingest Document

Generate Embeddings

Retrieve Context

Ask Question

Store Conversation

Run Evaluation
```

---

# Application Layer Responsibilities

Example:

```text
Question Arrives
```

Application coordinates:

```text
Generate Embedding
       ↓
Search Vector Store
       ↓
Retrieve Chunks
       ↓
Build Prompt
       ↓
Call LLM
       ↓
Save Conversation
```

---

# Why Not Put This In Controllers?

Because controllers should only handle:

```text
HTTP
```

not

```text
Business Workflows
```

Otherwise:

```text
Endpoints become huge.

Logic gets duplicated.

Testing becomes difficult.
```

---

# Infrastructure Layer

# Purpose

Infrastructure handles interaction with external systems.

Examples:

```text
Qdrant

Ollama

PDF Processing

Embedding Models

Storage

External Services
```

---

# Infrastructure Implements Contracts

Application depends upon abstractions.

Infrastructure provides implementations.

Example:

```text
IVectorStore
```

Domain/Application:

```text
Only knows interface
```

Infrastructure:

```text
QdrantVectorStore
```

implements the interface.

---

# Example

Instead of:

```csharp
new QdrantClient(...)
```

inside business logic,

we use:

```csharp
IVectorStore
```

Application depends on:

```text
IVectorStore
```

Infrastructure provides:

```text
QdrantVectorStore
```

---

# Why This Matters

Later we can switch from:

```text
Qdrant
```

to:

```text
Azure AI Search

Pinecone

Weaviate
```

without changing business workflows.

---

# Infrastructure Is Replaceable

Examples:

Current:

```text
Ollama
```

Future:

```text
Azure OpenAI
```

Application layer remains unchanged.

Only Infrastructure changes.

---

# API Layer

# Purpose

The API layer exposes the system to the outside world.

Examples:

```text
HTTP Endpoints

Request Validation

Response Formatting

Streaming

SSE
```

---

# API Should Not Contain

```text
Retrieval Logic

Prompt Engineering

Embedding Logic

Vector Search Logic
```

These belong elsewhere.

---

# API Responsibilities

Example:

```text
POST /conversation/stream
```

API should:

```text
Receive Request
       ↓
Validate Request
       ↓
Call Application Layer
       ↓
Stream Response
```

Nothing more.

---

# Why Thin Endpoints Are Better

Benefits:

```text
Easy Testing

Easy Maintenance

Less Duplication

Better Separation Of Concerns
```

---

# Dependency Rule

This is the most important rule.

Dependencies always point inward.

---

# Dependency Graph

```text
API
 ↓

Application
 ↓

Domain

Infrastructure
      ↑
      |
implements contracts
```

---

# Valid Dependency Direction

```text
API → Application

Application → Domain

Infrastructure → Domain

Infrastructure → Application Contracts
```

---

# Invalid Dependency Direction

```text
Domain → Infrastructure

Application → API

Domain → HTTP
```

These would break Clean Architecture.

---

# Dependency Inversion Principle

One of the core SOLID principles.

Instead of:

```text
Application
      ↓
Qdrant
```

we use:

```text
Application
      ↓
IVectorStore
      ↑
Implementation
```

---

# Why Dependency Inversion Exists

Without DIP:

```text
Business Logic
      ↓
Concrete Technologies
```

With DIP:

```text
Business Logic
      ↓
Contracts
      ↑
Concrete Implementations
```

This reduces coupling.

---

# Request Flow Through Layers

Example:

```text
Question:
What is self-attention?
```

---

# Step 1

API Layer receives request.

```text
POST /conversation/stream
```

---

# Step 2

Application layer handles:

```text
Ask Question Use Case
```

---

# Step 3

Application requests:

```text
Generate Embedding
```

through abstraction.

---

# Step 4

Infrastructure uses:

```text
Embedding Service
```

to create vectors.

---

# Step 5

Application requests:

```text
Retrieve Similar Chunks
```

through:

```text
IVectorStore
```

---

# Step 6

Infrastructure executes:

```text
Qdrant Search
```

---

# Step 7

Application:

```text
Builds Prompt

Adds Memory

Adds Context
```

---

# Step 8

Infrastructure calls:

```text
Ollama
```

---

# Step 9

Application returns token stream.

---

# Step 10

API streams tokens via:

```text
SSE
```

to browser.

---

# Architecture Diagram

```text
Browser UI
      │
      ▼

API Layer
      │
      ▼

Application Layer
      │
      ▼

Domain Layer

      ▲
      │

Infrastructure Layer
      │
      ├── Qdrant
      ├── Ollama
      ├── PDF Processing
      └── Embedding Services
```

---

# Why Not Use A Single Project?

A common assessment question.

---

# Small Prototype Approach

```text
Program.cs

Services/

Models/

Controllers/
```

Advantages:

```text
Fast
Simple
```

---

# Problems As System Grows

Eventually:

```text
Huge Controllers

Huge Services

Tight Coupling

Difficult Testing
```

---

# Why Clean Architecture Was Appropriate

Our system includes:

```text
Document Processing

Embeddings

Vector Retrieval

Conversation Memory

Evaluation

Diagnostics

Streaming API

Browser UI
```

This is large enough to justify architectural separation.

---

# Benefits We Actually Experienced

---

## Example 1

When we added:

```text
Conversation Memory
```

Did retrieval logic change?

```text
No
```

---

## Example 2

When we added:

```text
SSE Streaming
```

Did Qdrant logic change?

```text
No
```

---

## Example 3

When we added:

```text
Browser UI
```

Did domain logic change?

```text
No
```

---

## Example 4

When we added:

```text
Evaluation Framework
```

Did document ingestion change?

```text
No
```

---

These are practical demonstrations of loose coupling.

---

# Key Architectural Decisions

## ADR-001

```text
Adopt Clean Architecture
```

Reason:

```text
Separation Of Concerns
```

---

## ADR-002

```text
Use Qdrant
```

Reason:

```text
Vector Search
```

---

## ADR-003

```text
Use Ollama
```

Reason:

```text
Local Model Hosting
```

---

## ADR-004

```text
Use SSE
```

Reason:

```text
Streaming Responses
```

---

## ADR-005

```text
Host Lightweight Browser UI
```

Reason:

```text
Simple Deployment
```

---

# Common Assessment Questions

## Why Clean Architecture?

> To separate business rules from technology concerns and improve maintainability, testability, and extensibility.

---

## Why Dependency Inversion?

> Business logic should depend on abstractions rather than concrete implementations to reduce coupling and improve flexibility.

---

## Why Use Interfaces?

> Interfaces allow implementations such as Qdrant or Ollama to be replaced without changing business workflows.

---

## What Lives In Domain?

> Core business concepts, contracts, abstractions, entities, and rules that are independent of frameworks and technologies.

---

## What Lives In Application?

> Use cases, orchestration logic, and business workflows.

---

## What Lives In Infrastructure?

> External integrations such as Qdrant, Ollama, embeddings, PDF processing, and persistence.

---

## What Lives In API?

> HTTP concerns, validation, routing, DTOs, and response streaming.

---

## How Would You Replace Ollama?

> By creating a new infrastructure implementation while keeping contracts and application workflows unchanged.

---

## How Would You Replace Qdrant?

> By implementing the vector store abstraction with another vector database and registering the new implementation through dependency injection.

---

# Architecture Principles Learned

```text
Separation Of Concerns

Dependency Inversion

Abstraction Over Implementation

Loose Coupling

Technology Independence

Single Responsibility

Testability

Maintainability
```

---

# 60-Second Architecture Answer

> "The solution follows Clean Architecture with four primary layers: API, Application, Domain, and Infrastructure. The Domain layer contains business concepts and abstractions. The Application layer orchestrates use cases such as document ingestion, retrieval, prompt construction, and evaluation. The Infrastructure layer integrates with external technologies including Qdrant, Ollama, embeddings, and document processing. The API layer exposes HTTP and SSE endpoints. Dependencies always point inward, ensuring that business logic remains independent of frameworks and external systems. This architecture allowed us to add conversational memory, evaluation, diagnostics, streaming, and a browser UI without affecting core business workflows."