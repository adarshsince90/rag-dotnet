public static class ChatResponses
{
    public const string Greeting =
"""
Hello!

I can answer questions about the documents loaded into this system.

Try asking about:

- Transformers
- Self Attention
- BERT
- GPT
- Few-shot Learning

How can I help you today?
""";

public const string Capabilities =
"""
I am a Retrieval-Augmented Generation (RAG) assistant.

I can answer questions based on the documents loaded into this system.

Supported topics currently include:

- Transformers
- Attention Mechanisms
- BERT
- GPT
- Few-shot Learning

I also support conversational follow-up questions using conversation memory.
""";
}