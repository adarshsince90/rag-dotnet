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


## Current Retrieval Architecture

User
↓
API
↓
AskQuestionService
↓
IChunkProvider
↓
IRetriever
↓
RetrievalResponse
↓
Response DTOs
↓
API Response

---

## Retrieval Layer

The retriever is responsible for:

- Evaluating chunks
- Calculating scores
- Ranking results
- Applying Top-K selection
- Producing retrieval diagnostics

The retriever is not responsible for:

- HTTP concerns
- User-facing messages
- API formatting

---

## Current Retrieval Models

DocumentChunk

Contains:

- Id
- Content
- Source
- ChunkIndex

RetrievalResult

Contains:

- Chunk
- Score
- Rank

RetrievalResponse

Contains:

- Results
- Diagnostics

RetrievalDiagnostics

Contains:

- TotalChunks
- QualifiedChunks
- ReturnedChunks
- TopK

## Embedding Architecture

Document
↓
IChunkProvider
↓
DocumentChunk
↓
IEmbeddingGenerator
↓
Embedding Generation
↓
IChunkStore
↓
IRetriever
↓
Retrieval Results

---

## Current Infrastructure Components

Embeddings

- OllamaEmbeddingGenerator

Storage

- InMemoryChunkStore

Retrieval

- VectorRetriever
- RetrievalResultProcessor

## Current RAG Architecture

API
↓
QuestionAnsweringService

├── IRetriever
├── IPromptBuilder
└── IChatCompletionService

↓

Infrastructure

- VectorRetriever
- OllamaEmbeddingGenerator
- OllamaChatCompletionService

↓

Ollama

- nomic-embed-text
- gemma2:2b