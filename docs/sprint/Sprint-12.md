# Sprint 12 - Browser Chat UI

## Objectives

- Build a lightweight browser UI for the RAG system.
- Reuse existing SSE streaming endpoint.
- Support conversational memory.
- Display sources and diagnostics.

## Completed Work

### Static Web UI

- Added static HTML-based chat interface.
- Added chat-specific CSS styling.
- Added JavaScript-based SSE client.

### Streaming Responses

- Consumed `/conversation/stream`.
- Rendered tokens incrementally.
- Added typing indicator.
- Added streaming cursor.

### Conversation Management

- Persisted ConversationId in localStorage.
- Added New Chat button.
- Added ability to reset conversation context.

### RAG Visualisation

- Displayed source documents.
- Displayed retrieval diagnostics.
- Displayed response generation metrics.

## Architecture Impact

No changes to RAG pipeline.

UI consumes existing API endpoints without introducing new backend services.

## Challenges

- CORS troubleshooting.
- SSE parsing.
- Client-side event processing.
- Stream rendering UX.

## Outcome

Users can now interact with the RAG assistant directly from a browser without using Swagger, Scalar, curl, or the CLI client.