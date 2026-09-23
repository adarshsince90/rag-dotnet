const fs = require('fs');
const path = require('path');

const rootDir = path.resolve(__dirname, '..');
const docsDir = path.join(rootDir, 'docs');
const prepDir = path.join(docsDir, 'assessment-prep');
const adrDir = path.join(docsDir, 'adr');
const sprintDir = path.join(docsDir, 'sprint');
const conceptsDir = path.join(docsDir, 'concepts');
const portalDir = path.join(docsDir, 'portal');

if (!fs.existsSync(portalDir)) {
  fs.mkdirSync(portalDir, { recursive: true });
}

// 1. Parse 16 Assessment Prep Guides
const guides = [];
const guideFiles = fs.readdirSync(prepDir).filter(f => f.endsWith('.md')).sort();

const guideCategories = {
  '01': { phase: 'Phase 1: Foundations & Flow', icon: '🏛️', tag: 'Foundations', readTime: '8 min' },
  '02': { phase: 'Phase 1: Foundations & Flow', icon: '🔄', tag: 'Core Flow', readTime: '12 min' },
  '03': { phase: 'Phase 1: Foundations & Flow', icon: '🏗️', tag: 'Architecture', readTime: '14 min' },
  '04': { phase: 'Phase 2: Core Mechanics', icon: '🔢', tag: 'Vectors & Math', readTime: '10 min' },
  '05': { phase: 'Phase 2: Core Mechanics', icon: '✂️', tag: 'Chunking & Ingestion', readTime: '15 min' },
  '06': { phase: 'Phase 2: Core Mechanics', icon: '✍️', tag: 'Prompting & Grounding', readTime: '12 min' },
  '07': { phase: 'Phase 2: Core Mechanics', icon: '🧠', tag: 'Memory & State', readTime: '12 min' },
  '08': { phase: 'Phase 2: Core Mechanics', icon: '⚡', tag: 'Streaming & SSE', readTime: '13 min' },
  '09': { phase: 'Phase 3: Production & Quality', icon: '📊', tag: 'Evaluation & Metrics', readTime: '14 min' },
  '10': { phase: 'Phase 3: Production & Quality', icon: '🤖', tag: 'Advanced & Agentic', readTime: '16 min' },
  '11': { phase: 'Phase 3: Production & Quality', icon: '🚀', tag: 'Production Ops & SRE', readTime: '15 min' },
  '12': { phase: 'Phase 3: Production & Quality', icon: '🛡️', tag: 'Security & Threats', readTime: '15 min' },
  '13': { phase: 'Phase 4: Strategy & Assessment', icon: '⚖️', tag: 'Trade-offs & Lessons', readTime: '13 min' },
  '14': { phase: 'Phase 4: Strategy & Assessment', icon: '🎯', tag: 'Mock Q&A & Interview', readTime: '15 min' },
  '15': { phase: 'Phase 4: Strategy & Assessment', icon: '📑', tag: 'Quick Cheat Sheet', readTime: '6 min' },
  '16': { phase: 'Phase 4: Strategy & Assessment', icon: '🔮', tag: 'Future AI Evolution', readTime: '8 min' }
};

for (const file of guideFiles) {
  const filePath = path.join(prepDir, file);
  const content = fs.readFileSync(filePath, 'utf-8');
  const numPrefix = file.substring(0, 2);
  const lines = content.split('\n');
  const titleLine = lines.find(l => l.startsWith('# ')) || `# ${file}`;
  const title = titleLine.replace(/^#\s+/, '').trim();
  const meta = guideCategories[numPrefix] || { phase: 'General', icon: '📄', tag: 'Reference', readTime: '10 min' };

  guides.push({
    id: `guide-${numPrefix}`,
    file: file,
    number: numPrefix,
    title: title,
    phase: meta.phase,
    icon: meta.icon,
    tag: meta.tag,
    readTime: meta.readTime,
    content: content
  });
}

// 2. Parse Mock Q&A for Trainer Flashcards from Guide 14
const flashcards = [];
const guide14File = path.join(prepDir, '14-Mock-Assessment-Questions-And-Answers.md');
if (fs.existsSync(guide14File)) {
  const g14Text = fs.readFileSync(guide14File, 'utf-8');
  const sections = g14Text.split(/\n# /);
  
  for (let sIdx = 1; sIdx < sections.length; sIdx++) {
    const sec = sections[sIdx];
    const lines = sec.split('\n');
    const category = lines[0].trim();
    if (category.toLowerCase().includes('purpose')) continue;

    const qBlocks = sec.split(/\n## Q:\s*/);
    for (let qIdx = 1; qIdx < qBlocks.length; qIdx++) {
      const qBlock = qBlocks[qIdx];
      const parts = qBlock.split(/\n### Answer\s*\n/);
      if (parts.length >= 2) {
        const question = parts[0].trim().replace(/\n.*$/, '');
        let answer = parts.slice(1).join('\n### Answer\n').trim();
        answer = answer.replace(/\n---\s*$/, '').trim();
        
        flashcards.push({
          id: `card-${flashcards.length + 1}`,
          category: category,
          question: question,
          answer: answer
        });
      }
    }
  }
}

// 3. Parse ADRs
const adrs = [];
const adrFiles = fs.readdirSync(adrDir).filter(f => f.startsWith('ADR-') && f.endsWith('.md')).sort();

for (const file of adrFiles) {
  const content = fs.readFileSync(path.join(adrDir, file), 'utf-8');
  const matchNum = file.match(/ADR-(\d+)/);
  const num = matchNum ? `ADR-${matchNum[1]}` : file.replace('.md', '');
  
  const titleLine = content.split('\n').find(l => l.startsWith('# ')) || `# ${file}`;
  const title = titleLine.replace(/^#\s+/, '').trim();
  
  const statusMatch = content.match(/\*\*Status:\*\*\s*([^\n\r]+)/i) || content.match(/Status:\s*([^\n\r]+)/i);
  const status = statusMatch ? statusMatch[1].trim() : 'Accepted';

  const contextMatch = content.match(/##\s*Context[\r\n]+([\s\S]*?)(?=##|$)/i);
  const decisionMatch = content.match(/##\s*Decision[\r\n]+([\s\S]*?)(?=##|$)/i);

  adrs.push({
    id: num.toLowerCase(),
    code: num,
    title: title,
    status: status,
    context: contextMatch ? contextMatch[1].trim().substring(0, 300) + '...' : '',
    decision: decisionMatch ? decisionMatch[1].trim().substring(0, 300) + '...' : '',
    fullContent: content
  });
}

// 4. Parse Sprints
const sprints = [];
const sprintFiles = fs.readdirSync(sprintDir).filter(f => f.startsWith('Sprint-') && f.endsWith('.md')).sort();

for (const file of sprintFiles) {
  const content = fs.readFileSync(path.join(sprintDir, file), 'utf-8');
  const sprintId = file.replace('.md', '');
  const titleLine = content.split('\n').find(l => l.startsWith('# ')) || `# ${sprintId}`;
  const title = titleLine.replace(/^#\s+/, '').trim();

  const goalMatch = content.match(/##\s*Goal[\r\n]+([\s\S]*?)(?=##|$)/i) ||
                    content.match(/##\s*Objectives?[\r\n]+([\s\S]*?)(?=##|$)/i);

  sprints.push({
    id: sprintId.toLowerCase(),
    code: sprintId,
    title: title,
    goal: goalMatch ? goalMatch[1].trim().substring(0, 250) + '...' : 'Sprint implementation steps and deliverables',
    fullContent: content
  });
}

// 5. Parse Concepts
const concepts = [];
const conceptFiles = fs.readdirSync(conceptsDir).filter(f => f.endsWith('.md')).sort();

for (const file of conceptFiles) {
  const content = fs.readFileSync(path.join(conceptsDir, file), 'utf-8');
  const titleLine = content.split('\n').find(l => l.startsWith('# ')) || `# ${file}`;
  const title = titleLine.replace(/^#\s+/, '').trim();
  const summaryLine = content.split('\n').find(l => l.length > 20 && !l.startsWith('#')) || '';

  concepts.push({
    id: file.replace('.md', ''),
    title: title,
    summary: summaryLine.trim(),
    fullContent: content
  });
}

// System Metadata & Specs
const systemSpecs = {
  framework: ".NET 10 (C# 13 / C# 14)",
  architecture: "Clean Architecture (Domain, Application, Infrastructure, Api, Cli)",
  localLLM: "Ollama (Gemma 2 2B)",
  cloudLLM: "Groq Cloud (Llama 3.3 70B / mixtral-8x7b-32768)",
  vectorDb: "Qdrant (Cosine Similarity Distance, ragdemo-documents)",
  embeddingModel: "nomic-embed-text (768 dimensions)",
  pdfExtractor: "PdfPig (PdfDocumentExtractor)",
  chunkingStrategy: "CharacterChunkingStrategy (1,000 chars / 200 overlap)",
  streamingProtocol: "Server-Sent Events (SSE) via IAsyncEnumerable",
  conversationMemory: "InMemoryConversationMemory (ConcurrentDictionary, 4-turn sliding FIFO)",
  testSuite: "10 Core Scenarios + 2 Grounding Refusal Probes (100% Pass Rate)"
};

// Benchmark Scenarios for Interactive Browser Simulator
const benchmarks = [
  {
    id: "self-attention",
    category: "Definition",
    question: "What is self-attention?",
    icon: "🔹",
    isRefusal: false,
    enrichedQuery: "What is self-attention?",
    retrievedChunks: [
      {
        source: "attention-is-what-you-need.pdf (Chunk #14)",
        score: 0.884,
        text: "Self-attention, sometimes called intra-attention, is an attention mechanism relating different positions of a single sequence in order to compute a representation of the sequence. Self-attention has been used successfully in a variety of tasks including reading comprehension, abstractive summarization, textual entailment and learning task-independent sentence representations."
      },
      {
        source: "attention-is-what-you-need.pdf (Chunk #22)",
        score: 0.812,
        text: "An attention function can be described as mapping a query and a set of key-value pairs to an output, where the query, keys, values, and output are all vectors. The output is computed as a weighted sum of the values, where the weight assigned to each value is computed by a compatibility function of the query with the corresponding key."
      },
      {
        source: "attention-is-what-you-need.pdf (Chunk #28)",
        score: 0.745,
        text: "Multi-head attention allows the model to jointly attend to information from different representation subspaces at different positions. With a single attention head, averaging inhibits this."
      }
    ],
    systemPromptSnippet: "You are a factual AI assistant. Use ONLY the following context to answer the question. If the answer cannot be found in the context, say 'I cannot answer this question based on the provided documents.' Do not extrapolate or hallucinate.\n\n<context>\n[1] attention-is-what-you-need.pdf: Self-attention, sometimes called intra-attention, is an attention mechanism...\n</context>",
    answer: "Based on the provided document (*Attention Is All You Need*):\n\n**Self-attention** (also known as *intra-attention*) is an attention mechanism relating different positions of a single sequence to compute a unified representation of that sequence.\n\nKey characteristics from the paper:\n- **Mechanism**: Maps a query and key-value pairs to an output vector, computed as a weighted sum where each weight represents the query-key compatibility score.\n- **Applications**: Successfully utilized in reading comprehension, abstractive summarization, and task-independent sentence representations.\n- **Role in Transformer**: Replaces recurrence and convolution entirely, allowing the model to connect all sequence positions in $O(1)$ operations with full parallelization.",
    metrics: {
      ollama: { retrievalMs: 142, generationMs: 3820, tokensPerSec: 38, returnedChunks: 3, topScore: 0.884 },
      groq: { retrievalMs: 134, generationMs: 410, tokensPerSec: 285, returnedChunks: 3, topScore: 0.884 }
    }
  },
  {
    id: "transformer-architecture",
    category: "Architecture",
    question: "Summarize the Transformer architecture.",
    icon: "🏛️",
    isRefusal: false,
    enrichedQuery: "Summarize the Transformer architecture.",
    retrievedChunks: [
      {
        source: "attention-is-what-you-need.pdf (Chunk #18)",
        score: 0.892,
        text: "The Transformer follows this overall architecture using stacked self-attention and point-wise, fully connected layers for both the encoder and decoder. The encoder is composed of a stack of N = 6 identical layers. Each layer has two sub-layers: a multi-head self-attention mechanism and a simple, position-wise fully connected feed-forward network."
      },
      {
        source: "attention-is-what-you-need.pdf (Chunk #19)",
        score: 0.835,
        text: "The decoder is also composed of a stack of N = 6 identical layers. In addition to the two sub-layers in each encoder layer, the decoder inserts a third sub-layer, which performs multi-head attention over the output of the encoder stack. We also employ residual connections around each of the sub-layers, followed by layer normalization."
      }
    ],
    systemPromptSnippet: "You are a factual AI assistant. Use ONLY the following context to answer the question...\n\n<context>\n[1] attention-is-what-you-need.pdf: The Transformer follows this overall architecture using stacked self-attention...\n</context>",
    answer: "Based on *Attention Is All You Need*, the Transformer architecture consists of an **Encoder-Decoder** structure composed of stacked attention and feed-forward layers:\n\n1. **Encoder ($N = 6$ identical layers)**:\n   - **Multi-Head Self-Attention**: Captures pairwise relationships across all sequence positions.\n   - **Position-wise Feed-Forward Network**: Fully connected layer applied separately to each position.\n   - Residual connections and layer normalization (`LayerNorm(x + Sublayer(x))`) wrap each sub-layer.\n\n2. **Decoder ($N = 6$ identical layers)**:\n   - Adds a third sub-layer performing multi-head attention over the encoder's output representations.\n   - Incorporates causal masking in self-attention to prevent positions from attending to future tokens.\n\n3. **Positional Encodings**:\n   - Fixed sinusoidal signals added to input embeddings to inject positional information without recurrence.",
    metrics: {
      ollama: { retrievalMs: 148, generationMs: 4100, tokensPerSec: 36, returnedChunks: 2, topScore: 0.892 },
      groq: { retrievalMs: 138, generationMs: 440, tokensPerSec: 280, returnedChunks: 2, topScore: 0.892 }
    }
  },
  {
    id: "transformer-vs-rnns",
    category: "Relationship",
    question: "What are the advantages of transformers over RNNs?",
    icon: "⚡",
    isRefusal: false,
    enrichedQuery: "What are the advantages of transformers over RNNs?",
    retrievedChunks: [
      {
        source: "attention-is-what-you-need.pdf (Chunk #8)",
        score: 0.876,
        text: "Recurrent models typically factor computation along the symbol positions of the input and output sequences. Aligning the positions to steps in computation time, they generate a sequence of hidden states h_t, as a function of the previous hidden state h_{t-1} and the input. This inherently sequential nature precludes parallelization within training examples."
      },
      {
        source: "attention-is-what-you-need.pdf (Chunk #10)",
        score: 0.854,
        text: "The Transformer allows for significantly more parallelization and can reach a new state of the art in translation quality after being trained for as little as twelve hours on eight P100 GPUs. Self-attention layers connect all positions with a constant number of sequentially executed operations, whereas recurrent layers require O(n) sequential operations."
      }
    ],
    systemPromptSnippet: "You are a factual AI assistant. Use ONLY the following context to answer the question...\n\n<context>\n[1] attention-is-what-you-need.pdf: Recurrent models typically factor computation along the symbol positions...\n</context>",
    answer: "Based on the indexed document, the primary advantages of Transformers over RNNs include:\n\n1. **High Parallelization**: RNNs are strictly sequential because computing state $h_t$ requires previous state $h_{t-1}$, preventing parallelization during training. Transformers compute attention over all token positions simultaneously.\n2. **Constant Maximum Path Length**: Self-attention connects any two token positions in $O(1)$ sequential operations, compared to $O(n)$ in recurrent layers. This drastically mitigates the vanishing gradient problem over long-range dependencies.\n3. **Training Speed & Efficiency**: Reaches state-of-the-art translation quality with substantially lower training compute (12 hours on 8 P100 GPUs vs. weeks for recurrent ensembles).",
    metrics: {
      ollama: { retrievalMs: 139, generationMs: 3650, tokensPerSec: 39, returnedChunks: 2, topScore: 0.876 },
      groq: { retrievalMs: 132, generationMs: 390, tokensPerSec: 290, returnedChunks: 2, topScore: 0.876 }
    }
  },
  {
    id: "refusal-probe",
    category: "Grounding Probe",
    question: "Refusal Probe: What is the capital of France?",
    icon: "🛡️",
    isRefusal: true,
    enrichedQuery: "Refusal Probe: What is the capital of France?",
    retrievedChunks: [
      {
        source: "attention-is-what-you-need.pdf (Candidate Chunk)",
        score: 0.285,
        text: "[Below Similarity Threshold - No Semantic Relevance]: ...the model achieved 28.4 BLEU on the WMT 2014 English-to-German translation task..."
      }
    ],
    systemPromptSnippet: "You are a factual AI assistant. Use ONLY the following context to answer the question. If the answer cannot be found in the context, say 'I cannot answer this question based on the provided documents.' Do not extrapolate or hallucinate.\n\n<context>\n(No relevant chunks passed the similarity threshold of 0.650)\n</context>",
    answer: "I cannot answer this question based on the provided documents.\n\nThe indexed corpus exclusively covers transformer neural network architectures, attention mechanisms, and pretraining methodologies (*Attention Is All You Need*, *BERT*, *Few-Shot Learners*). There is no contextual information regarding the capital of France, and ungrounded generation is prohibited by system design.",
    metrics: {
      ollama: { retrievalMs: 22, generationMs: 320, tokensPerSec: 42, returnedChunks: 0, topScore: 0.285 },
      groq: { retrievalMs: 18, generationMs: 95, tokensPerSec: 310, returnedChunks: 0, topScore: 0.285 }
    }
  }
];

// Write output as JavaScript window bundle
const jsOutput = `// Auto-generated RAG System Knowledge Bundle
// Generated: ${new Date().toISOString()}

window.RAG_DATA = {
  specs: ${JSON.stringify(systemSpecs, null, 2)},
  guides: ${JSON.stringify(guides, null, 2)},
  flashcards: ${JSON.stringify(flashcards, null, 2)},
  adrs: ${JSON.stringify(adrs, null, 2)},
  sprints: ${JSON.stringify(sprints, null, 2)},
  concepts: ${JSON.stringify(concepts, null, 2)},
  benchmarks: ${JSON.stringify(benchmarks, null, 2)}
};
`;

const outputPath = path.join(portalDir, 'data.js');
fs.writeFileSync(outputPath, jsOutput, 'utf-8');
console.log('Successfully compiled data bundle to ' + outputPath);
console.log('- ' + guides.length + ' Master Guides');
console.log('- ' + flashcards.length + ' Mock Q&A Flashcards');
console.log('- ' + adrs.length + ' ADRs');
console.log('- ' + sprints.length + ' Sprints');
console.log('- ' + concepts.length + ' Concepts');
