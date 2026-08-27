Evaluation Findings
Evaluation Summary

The RAG chatbot was evaluated using a benchmark dataset containing 12 evaluation scenarios covering:

Definitions
Relationships
Concepts
Research Paper Questions
Conversational Memory
Grounding

The evaluation framework executes all questions through the production conversational pipeline, including:

Retrieval
Conversation Memory
Prompt Building
Grounded Answer Generation
Evaluation Scoring
Final Results
Core Metrics
Total Questions              : 12

Expected Source Coverage     : 100.0%

Average Keyword Coverage     : 91.7%

Grounding Accuracy           : 100.0%

Average Retrieval Time       : 145 ms

Average Generation Time      : 43.0 sec

Average Similarity Score     : 0.67

Approximate RAG Metrics
Faithfulness                 : 100.0%

Answer Correctness           : 91.7%

Context Recall               : 100.0%

Context Precision            : 67.0%

Category-Level Observations
Definition Questions

Questions included:

What is attention?
What is self-attention?
What is BERT?

Findings
Retrieved the correct source documents consistently.
Generated accurate and grounded definitions.
Demonstrated strong semantic retrieval despite wording variations.
Outcome

✅ Successful

Relationship Questions

Questions included:

How does attention help transformers?
What are the advantages of transformers over RNNs?

Findings
Successfully combined information across multiple retrieved chunks.
Generated explanations describing relationships rather than simple definitions.
Demonstrated the effectiveness of retrieval grounded reasoning.
Outcome

✅ Successful

Concept Questions

Questions included:

What is in-context learning?
What is few-shot learning?

Findings
Retrieval consistently identified the correct source document.
Answers contained the expected concepts and terminology.
High keyword coverage achieved.
Outcome

✅ Successful

Paper Questions

Questions included:

What are the main contributions of BERT?
What are the main contributions of the Transformer paper?

Findings
Retrieval successfully located relevant research papers.
Generated answers accurately summarized key contributions.
BERT contribution questions were more challenging due to overlapping information across multiple documents.
Outcome

✅ Successful with minor variability in keyword coverage.

Conversational Memory Questions

Questions included:

What is attention?
How does it help transformers?


and

What is BERT?
How does it differ from GPT?

Findings
Memory-enabled retrieval successfully handled follow-up questions.
Pronoun resolution worked correctly in conversational scenarios.
Entity-resolution style questions remain more difficult than direct references.
Outcome

✅ Successful

Grounding Questions

Question:

What is human attention?

Findings
The answer was not present in the document corpus.
The chatbot correctly refused to generate unsupported information.
No hallucination was observed.

Expected response:

I could not find the answer in the provided documents.

Outcome

✅ Successful

Key Improvements Identified During Evaluation
Improved PDF Extraction

The original extraction approach produced merged words.

Example:

Thedominantsequencetransductionmodels...


The extraction pipeline was improved using word-level reconstruction.

Result:

The dominant sequence transduction models...


Impact:

Better chunk quality
Better embeddings
Improved retrieval
Improved answer generation
Source Coverage Evaluation

Initial evaluation relied on exact source matching.

This proved too restrictive because correct answers can be generated using multiple relevant documents.

The metric was updated to:

Expected Source Coverage


which measures whether the expected source document appeared in the retrieved source list.

Impact:

More realistic retrieval evaluation.
Better alignment with RAG system behavior.
Grounding Improvements

Prompt instructions were strengthened to ensure answers are generated only from retrieved document content.

Additional safeguards were introduced for:

Greeting handling
Out-of-domain questions
Unsupported topics

Impact:

Reduced hallucinations.
Improved faithfulness.
Improved consistency.
Performance Analysis
Retrieval Performance
Average Retrieval Time: 145 ms


Retrieval performance remained consistently fast throughout testing.

The vector retrieval layer is not a bottleneck.

Generation Performance
Average Generation Time: 43 sec


Generation accounts for the majority of response latency.

Observation:

Retrieval ≈ milliseconds
Generation ≈ several seconds


The primary bottleneck is local LLM inference through Ollama.

Challenges Encountered
PDF Extraction Quality

Academic PDFs initially produced poor text extraction quality.

This directly affected:

Retrieval
Keyword coverage
Grounding

The issue was mitigated through improved extraction.

Conversational Entity Resolution

Questions such as:

How does it differ from GPT?


remain more challenging than direct follow-up questions because they require entity disambiguation before retrieval.

Future query rewriting strategies may improve these scenarios.

Local LLM Resource Constraints

During large evaluation runs, Ollama occasionally reported:

model runner has unexpectedly stopped


This was attributed to local resource limitations rather than application logic.

Overall Assessment

The final system successfully achieved:

✅ 100% Context Recall

✅ 100% Faithfulness

✅ 91.7% Answer Correctness

✅ 100% Grounding Accuracy

---

The evaluation demonstrates that the chatbot can reliably:

Retrieve relevant information
Generate grounded responses
Maintain conversational context
Avoid hallucinations
Answer document-based questions accurately

The primary area for future improvement is retrieval precision and conversational entity-resolution, while overall system performance and answer quality are strong.

---
Ollama Results

Faithfulness       : 100%
Answer Correctness : 91.7%
Context Recall     : 100%
Context Precision  : 67%

Average Retrieval  : 145 ms
Average Generation : 43 sec

Groq Results
Faithfulness       : 100%
Answer Correctness : 80%
Context Recall     : 100%
Context Precision  : 67.7%

Average Retrieval  : 132 ms
Average Generation : 1.6 sec