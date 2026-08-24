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

---
## Sprint 5B

The bottleneck of a RAG system is not always retrieval.

For document-heavy systems:

Document Processing
  ↓
Chunking
  ↓
Embedding Generation
  ↓
Storage

often costs significantly more than query-time retrieval.

This motivates persistent vector storage.

### Architectural Validation

Sprint 5B demonstrated the value of separation of concerns.

Only the document acquisition layer changed:

TXT
↓
TextFileChunkProvider

became

PDF
↓
PdfDocumentExtractor
↓
PdfChunkProvider

The following components remained unchanged:

- Chunking Strategies
- Embedding Generation
- InMemoryChunkStore
- VectorRetriever
- Prompt Builder
- Question Answering Service
- Chat Completion Service

This validated that document ingestion was correctly isolated from retrieval and generation concerns.

---

## Vector Database Layer

Sprint 6 introduced Qdrant as the vector database.

Architecture:

Question
↓
VectorRetriever
↓
Qdrant
↓
Retrieved Chunks
↓
Prompt Builder
↓
LLM

Responsibilities:

Qdrant:

- Persistent Vector Storage
- Approximate Nearest Neighbour Search
- Metadata Storage

VectorRetriever:

- Query Embedding Generation
- Retrieval Orchestration

RetrievalResultProcessor:

- Similarity Filtering
- Ranking
- Diagnostic Calculations

This separation preserves clean architecture boundaries while enabling future retrieval enhancements.

---

# Conversational RAG Architecture

Sprint 7B introduced conversational memory capabilities.

The system now supports:

- Single-turn RAG
- Multi-turn Conversational RAG
- Streaming Responses

---

## Standard RAG Flow

Question
↓
Retrieval
↓
Prompt Construction
↓
LLM
↓
Response

Endpoints:

POST /ask

POST /ask/stream

---

## Conversational RAG Flow

ConversationId
+
Current Question
↓
Load Conversation History
↓
Build Retrieval Query
(Previous Questions + Current Question)
↓
Qdrant Retrieval
↓
Prompt Construction
(History + Retrieved Context)
↓
LLM
↓
Store Interaction
↓
Streaming Response

Endpoint:

POST /ask/conversation/stream

---

## Memory Architecture

Conversation Memory is independent from document retrieval.

Conversation Memory:

Stores:

- User Questions
- Assistant Responses
- Timestamp

Purpose:

Provide conversational continuity.

---

Knowledge Memory:

Qdrant

Stores:

- Document Chunks
- Embeddings
- Metadata

Purpose:

Provide knowledge retrieval.

---

The application now combines:

Conversation Memory
+
Knowledge Memory

to generate responses.