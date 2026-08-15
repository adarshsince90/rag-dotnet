# Architecture

## Overview

The application follows a layered architecture that separates API concerns, application workflows, business contracts, and infrastructure implementations.

The primary goal is to allow the RAG system to evolve incrementally from simple keyword retrieval to embeddings, vector search, PDF processing, open-source LLM integration, conversational memory, and evaluation without significant refactoring.

---

## Layer Responsibilities

### API Layer (RagDemo.Api)

Responsible for:

- HTTP endpoints
- Request/response handling
- Swagger/OpenAPI
- Input validation
- Dependency injection configuration

The API layer should not contain business logic or infrastructure implementation details.

Example:

```text
POST /ask

## Current Retrieval Flow

Question
↓
Application Service
↓
Chunk Provider
↓
Retriever
↓
Retrieval Result
↓
Response

Application owns the workflow.

Infrastructure owns the implementation.

Domain owns the contracts.

API owns the transport.

## Current Implementations

IChunkProvider
→ TextFileChunkProvider

IRetriever
→ KeywordRetriever