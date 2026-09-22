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

// Write output as JavaScript window bundle
const jsOutput = `// Auto-generated RAG System Knowledge Bundle
// Generated: ${new Date().toISOString()}

window.RAG_DATA = {
  specs: ${JSON.stringify(systemSpecs, null, 2)},
  guides: ${JSON.stringify(guides, null, 2)},
  flashcards: ${JSON.stringify(flashcards, null, 2)},
  adrs: ${JSON.stringify(adrs, null, 2)},
  sprints: ${JSON.stringify(sprints, null, 2)},
  concepts: ${JSON.stringify(concepts, null, 2)}
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
