RagDemo.Infrastructure
│
├── Retrieval
│   ├── TextFileChunkProvider.cs
│   ├── KeywordRetriever.cs
│   └── VectorRetriever.cs
│
├── Embeddings
│   ├── Ollama
│   │   ├── OllamaEmbeddingGenerator.cs
│   │   ├── OllamaOptions.cs
│   │   └── OllamaEmbeddingResponse.cs
│   │
│   └── OpenAI
│       ├── OpenAiEmbeddingGenerator.cs
│       └── OpenAiOptions.cs
│
├── Chat
│   ├── Ollama
│   │   ├── OllamaChatModel.cs
│   │   └── OllamaChatResponse.cs
│   │
│   └── OpenAI
│       └── OpenAiChatModel.cs
│
└── Documents
    ├── Text
    │   └── TextFileChunkProvider.cs
    │
    └── Pdf
        └── PdfChunkProvider.cs

-------------------------------------
What Production Systems Often Do

Instead of:

C#
1
score > 0.7
Show more lines

they use multiple signals:

Signal 1

Similarity score.

Signal 2

Difference between Rank 1 and Rank 2.

Signal 3

Difference between Rank 1 and Median score.

Signal 4

Presence of exact entity matches.

---------------------------------
Query	Best ScoreWhere are headquarters?	0.82
Company location	0.79
Founded in?	0.88
Google CEO	0.50
Weather today	0.44
Capital of France	0.42