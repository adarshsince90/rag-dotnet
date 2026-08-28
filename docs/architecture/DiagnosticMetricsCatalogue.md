# Diagnostic Metrics Catalogue

The application emits diagnostic metrics at each stage of the RAG pipeline. These are returned as part of the SSE streaming response.

## Conversation Metrics

| Metric | Description |
|--------|-------------|
| `TurnsUsed` | Number of conversation history turns included |
| `HistoryCharacters` | Total characters in conversation history |
| `RetrievalQueryCharacters` | Characters in the query sent to retrieval |

## Retrieval Metrics

| Metric | Description |
|--------|-------------|
| `RetrievalMs` | Time spent on retrieval (ms) |
| `ReturnedChunks` | Number of chunks returned after filtering |
| `AverageScore` | Average similarity score of returned chunks |
| `HighestScore` | Highest similarity score |
| `LowestScore` | Lowest similarity score |
| `Sources` | Distinct source documents in results |

## Prompt Metrics

| Metric | Description |
|--------|-------------|
| `ContextCharacters` | Total characters in the retrieved context |
| `PromptCharacters` | Total characters in the final prompt |
| `RetrievedChunkCount` | Number of chunks in the prompt |

## Generation Metrics

| Metric | Description |
|--------|-------------|
| `GenerationMs` | LLM response generation time (ms) |
| `TotalMs` | Total end-to-end time (ms) |
| `AnswerCharacters` | Characters in the generated answer |

## Future Metrics

| Metric | Description |
|--------|-------------|
| `PromptTokens` | Token count of the prompt |
| `CompletionTokens` | Token count of the response |
| `TokensPerSecond` | Generation throughput |
