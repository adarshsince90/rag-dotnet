## Console Chat Client

Start the API:

```bash
dotnet run --project src/RagDemo.Api
```

In a separate terminal:

```bash
dotnet run --project src/RagDemo.Cli
```

Available commands:

```text
/new   Start a new conversation
/exit  Exit the application
```

Example:

```text
You: What is self-attention?

Assistant:
Self-attention is an attention mechanism...

Retrieval
--------------------------------
Time           : 78 ms
Chunks         : 5
Highest Score  : 0.72

Sources:
 • attention-is-what-you-need.pdf
```