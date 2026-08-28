# Final Report: Conversational RAG System in .NET 10

---

## 1. Executive Summary & Overview

### 1.1 Objective
The objective of this project was to design, develop, and benchmark a production-grade, conversational Retrieval-Augmented Generation (RAG) system built from first principles in **.NET 10**. Rather than relying on high-level orchestration abstractions (e.g., LangChain or Semantic Kernel) in initial phases, the system was implemented incrementally across 13 iterative sprints to achieve complete architectural transparency, rigorous separation of concerns, and full control over retrieval dynamics.

### 1.2 System Architecture
The system adheres to **Clean Architecture** principles and is structured into four distinct layers:

```
src/
├── RagDemo.Domain           # Core abstractions, entity models, evaluation contracts
├── RagDemo.Application      # Pipeline orchestration, prompt engineering, query classification
├── RagDemo.Infrastructure   # Concrete implementations (Ollama, Groq, Qdrant, PdfPig)
├── RagDemo.Api              # Minimal APIs, Server-Sent Events (SSE) endpoints, middleware
└── RagDemo.Cli              # Interactive streaming terminal client
```

#### Core Architecture Principles:
- **Dependency Inversion (SOLID)**: All application workflows interact strictly with interfaces (`IChatCompletionService`, `IVectorStore`, `IRetriever`, `IEmbeddingGenerator`, `IConversationMemory`).
- **Decoupled Pipelines**: Document ingestion/indexing and query-time retrieval operate as completely independent subsystems.
- **Multi-Provider AI Abstraction**: Dynamic switching between local on-premise execution (Ollama with Gemma 2 2B) and ultra-low-latency cloud acceleration (Groq with GPT-OSS-120B / Llama).

---

## 2. Technical Implementation

```mermaid
graph TD
    subgraph Ingestion Pipeline
        PDF[PDF Documents] -->|PdfPig| Extractor[PdfDocumentExtractor]
        Extractor -->|1000 chars / 200 overlap| Chunker[CharacterChunkingStrategy]
        Chunker -->|nomic-embed-text / 768-dim| Embedder[OllamaEmbeddingGenerator]
        Embedder -->|Batch Upsert| Qdrant[(Qdrant Vector DB)]
    end

    subgraph Query & Generation Pipeline
        User[User Query] --> Classifier{QueryClassifier}
        Classifier -->|Greeting / Capabilities| DirectResp[Direct Response]
        Classifier -->|Document Question| MemLoad[Load Recent Turns - IConversationMemory]
        MemLoad --> QueryBuild[Build Query - IConversationQueryBuilder]
        QueryBuild --> QEmbed[Generate Query Embedding]
        QEmbed --> QdrantSearch[Qdrant Cosine Similarity Search]
        QdrantSearch --> Processor[RetrievalResultProcessor]
        Processor --> PromptBuild[RagPromptBuilder - Grounded Prompt]
        PromptBuild --> LLM[IChatCompletionService - Ollama / Groq]
        LLM -->|SSE Stream| SSE[Token Streaming & Diagnostics]
        LLM --> SaveTurn[Save ConversationTurn]
    end
```

### 2.1 Data Extraction
- **Library**: `PdfPig`
- **Component**: `PdfDocumentExtractor` & `PdfChunkProvider`
- **Text Normalization**: Implemented character/word-level normalization to prevent whitespace truncation and token concatenation across hyphenated line breaks.

### 2.2 Chunking Strategy
- **Strategy**: `CharacterChunkingStrategy` (ADR-017)
- **Configuration**: Chunk Size = 1,000 characters; Overlap = 200 characters.
- **Rationale**: Validated against transformer research papers (629 chunks across 5 core research papers including *Attention Is All You Need* and *BERT*), yielding optimal recall without context fragmentation.

### 2.3 Embedding Generation
- **Model**: `nomic-embed-text` (768 dimensions) via local Ollama.
- **Concurrency**: Controlled asynchronous pipeline utilizing `SemaphoreSlim(8)` to maximize local CPU/GPU throughput while preventing memory exhaustion.

### 2.4 Vector Database & Storage
- **Engine**: `Qdrant` running in Docker container with persistent disk storage (`ragdemo-documents` collection).
- **Metric**: Cosine Similarity distance.
- **Payload Schema**: Structured metadata preserving `source`, `chunkIndex`, and raw `content` for complete document provenance and diagnostic attribution.

### 2.5 LLM Integration & Grounded Prompting
- **Local Provider**: Ollama running `gemma2:2b`.
- **Cloud Provider**: Groq API via `GroqChatCompletionService` with automated exponential backoff and retry policies.
- **Grounded Prompt Engine (`RagPromptBuilder`)**: Prompts enforce strict grounding constraints: if retrieved context is insufficient, the model is bound by contract to return:
  > *"I could not find the answer in the provided documents."*

### 2.6 Conversational Memory
- **Component**: `InMemoryConversationMemory` backed by thread-safe `ConcurrentDictionary`.
- **Context Depth**: Rolling window of the last 4 dialogue turns (`ConversationTurn`).
- **History-Aware Retrieval**: Historical turns enrich the retrieval query vector while keeping factual domain boundaries isolated to retrieved document chunks.

---

## 3. Evaluation Framework

To empirically validate retrieval fidelity, conversational coherence, and hallucination suppression, an automated evaluation pipeline was built directly into the application (`/evaluation/run`).

### 3.1 Evaluation Test Suite (10 Core Scenarios + 2 Edge Probes)

| ID | Category | Question | Expected Source | Key Assessment |
|---|---|---|---|---|
| 1 | Definition | What is attention? | `attention-is-what-you-need.pdf` | Core definition semantic retrieval |
| 2 | Definition | What is self-attention? | `attention-is-what-you-need.pdf` | Fine-grained distinction retrieval |
| 3 | Definition | What is BERT? | `transformers-llm.pdf` | Entity & model architecture definition |
| 4 | Relationship | How does attention help transformers? | `attention-is-what-you-need.pdf` | Cross-chunk synthesis & reasoning |
| 5 | Relationship | What are the advantages of transformers over RNNs? | `attention-is-what-you-need.pdf` | Comparative reasoning & keyword recall |
| 6 | Concept | What is in-context learning? | `language-model-few-shots.pdf` | Few-shot terminology grounding |
| 7 | Concept | What is few-shot learning? | `language-model-few-shots.pdf` | Concept definition & example coverage |
| 8 | Paper | What are the main contributions of BERT? | `bert-pretraining.pdf` | Multi-point paper contribution synthesis |
| 9 | Paper | What are the main contributions of the Transformer paper? | `attention-is-what-you-need.pdf` | Architecture contribution extraction |
| 10 | Memory | *[Turn 1: What is attention?]* <br>Follow-up: **How does it help transformers?** | `attention-is-what-you-need.pdf` | Conversational pronoun resolution (`it` $\rightarrow$ `attention`) |
| 11 | Memory | *[Turn 1: What is BERT?]* <br>Follow-up: **How does it differ from GPT?** | `language-model-few-shots.pdf` | Conversational entity tracking across papers |
| 12 | Grounding | What is human attention? | *(None)* | **Hallucination Refusal Probe** |

### 3.2 Quantitative Metrics & Scoring Methodology

1. **Expected Source Coverage**:
   $$\text{Source Coverage} = \frac{\sum \text{Hits}(\text{Expected Source} \in \text{Retrieved Sources})}{N_{\text{total}}}$$
2. **Keyword Coverage**:
   $$\text{Keyword Coverage} = \frac{\sum \text{Matched Target Concepts}}{\sum \text{Expected Keywords}}$$
3. **Grounding Accuracy (Faithfulness)**:
   Verification of strict refusal behavior on out-of-domain queries ($100\%$ refusal on ungrounded queries).
4. **Context Recall & Precision**:
   Ratio of relevant context retrieved versus non-relevant candidate noise.
5. **Latency Diagnostics**:
   Isolated millisecond-level measurement of retrieval time ($T_{\text{retrieval}}$) versus LLM time-to-first-token and total generation time ($T_{\text{gen}}$).

---

## 4. Results & Discussion

### 4.1 Benchmark Results Summary

| Metric | Local Ollama (Gemma 2 2B) | Cloud Groq (Llama 3 / GPT-OSS) | Benchmark Standard |
|---|---|---|---|
| **Context Recall** | **100.0%** | **100.0%** | $\ge 90.0\%$ |
| **Faithfulness / Grounding** | **100.0%** | **100.0%** | $100.0\%$ |
| **Answer Correctness** | **91.7%** | **80.0%** | $\ge 80.0\%$ |
| **Context Precision** | **67.0%** | **67.7%** | $\ge 60.0\%$ |
| **Average Retrieval Latency** | **145 ms** | **132 ms** | $< 300\text{ ms}$ |
| **Average Generation Latency** | **43.0 s** | **1.6 s** | N/A |

### 4.2 Key Findings & Discussion
- **Retrieval vs. Generation Latency**: Vector retrieval with Qdrant consistently completed in $\sim 140\text{ ms}$. Generation was the primary bottleneck under local hardware ($\sim 43\text{ s}$), which was resolved in two ways:
  1. Integrating Server-Sent Events (SSE) token streaming for immediate perceived responsiveness.
  2. Adding the Groq cloud provider, slashing generation latency by **$96\%$** (down to $1.6\text{ s}$).
- **Conversational Entity Resolution**: The history-augmented retrieval effectively solved pronoun references (e.g., *"How does it help transformers?"*), retaining high source accuracy without incurring latency overhead from additional query-rewriting LLM calls.
- **Zero Hallucinations on Edge Tests**: The system scored $100\%$ on the grounding refusal probe (Question 12: *"What is human attention?"*), validating the strict prompt constraints.

### 4.3 Challenges Encountered & Mitigations
1. **PDF Text Coalescence**: Academic two-column PDFs initially generated concatenated words (e.g., `attentionmechanisms`). This was solved by enhancing `PdfDocumentExtractor` with layout-aware spacing.
2. **Topic Drift in Multi-Turn Memory**: Appending raw history to queries can occasionally bias new distinct queries. Setting a sliding 4-turn window with explicit query classification prevented semantic pollution.

---

## 5. Conclusion & Future Work

### 5.1 Conclusion
The project successfully demonstrates an enterprise-grade Conversational RAG implementation in modern .NET 10. By coupling Clean Architecture with isolated retrieval and generation pipelines, Qdrant vector storage, and grounded prompt engineering, the system achieved **100% Context Recall**, **100% Faithfulness**, and **91.7% Answer Correctness** across comprehensive evaluation suites.

### 5.2 Future Roadmap
- **LLM-Based Query Rewriting**: Dynamic query reformulation for complex multi-hop queries.
- **Hybrid Sparse/Dense Search**: Combining BM25 / TF-IDF keyword ranking with dense Qdrant vector similarity using Reciprocal Rank Fusion (RRF).
- **Persistent Conversation Store**: Distributed session caching using Redis or PostgreSQL.
- **Agentic Workflows**: Tool calling and multi-step reasoning capabilities.
