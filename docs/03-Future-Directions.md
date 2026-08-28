# Future Directions

Planned enhancements and exploration areas for the RAG system, identified through sprint retrospectives and experimentation.

## Retrieval Enhancements

- **Query rewriting** — Use the LLM to reformulate queries for better retrieval
- **Query classification** — Dynamic retrieval strategies based on question type
- **Hybrid retrieval** — Combine keyword + semantic search with ranking fusion
- **Dynamic TopK / SearchLimit** — Adjust retrieval parameters per query
- **Metadata filtering** — Filter by source document, date, or section
- **Similarity threshold tuning** — Experiment with different minimum scores

## Memory Enhancements

- **Persistent memory** — Survive application restarts
- **Semantic memory** — Retrieve relevant past conversations
- **Memory summarization** — Compress long conversation histories
- **Topic shift detection** — Detect when user changes topic to avoid bias
- **Memory relevance scoring** — Only include relevant history in retrieval

## Generation Enhancements

- **Source citations in answers** — Inline references to source documents
- **Token counting** — Track prompt/completion tokens for cost monitoring
- **Response quality scoring** — Automated quality assessment

## AI Provider Expansion

- **Gemini provider** — Partially configured in appsettings
- **OpenAI-compatible providers** — Generic OpenAI API adapter

## Architecture

- **Tool calling** — Function calling for structured actions
- **Agents** — ReAct pattern, planning, reasoning loops
- **Agent workflows** — State machines, durable execution
- **Multi-agent systems** — Supervisor + specialized agents
- **Asynchronous document indexing** — Background ingestion

## Observability

- **Tracing** — End-to-end request tracing
- **Latency monitoring** — Per-stage timing dashboards
- **Token-per-second metrics** — Generation throughput tracking

---

## Related

- [Learning Roadmap](01-Learning-Roadmap.md) — Learning stage progression
- [Implementation Roadmap](02-Implementation-Roadmap.md) — Completed sprint history
- [Evaluation Findings](../data/evaluation/evaluation-findings.md) — Issues identified during evaluation
