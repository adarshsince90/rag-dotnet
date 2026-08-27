namespace RagDemo.Application.Constants;

public static class PromptConstants
{
   public const string NotFoundMarker =
        "I could not find the answer in the provided documents.";

    public const string NotFoundResponse =
"""
I could not find the answer in the provided documents.
Supported topics currently include:
- Transformers
- Attention Mechanisms
- BERT
- GPT
- Few-shot Learning
""";
}