# Sprint 9A - Refactoring

Enhance evaluation and query classification features

- Removed old evaluation results JSON file to clean up unused data.
- Added IQueryClassifier interface and QueryClassifier implementation for classifying user queries.
- Introduced new greeting and capabilities responses for user interaction.
- Updated ConversationRequestContext to manage retrieval and conversation persistence flags.
- Enhanced ConversationQuestionAnsweringService to handle direct responses and classify queries.
- Improved prompt building logic to clarify response expectations.
- Adjusted evaluation summary to include new metrics for faithfulness, answer correctness, context recall, and context precision.
- Updated appsettings to modify MinimumSimilarity threshold for retrieval.
- Implemented CORS policy to allow requests from the web UI.