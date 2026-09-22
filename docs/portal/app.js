/**
 * RAG .NET 10 System Walkthrough Portal - Application Logic
 */

document.addEventListener('DOMContentLoaded', () => {
  // Check that data bundle is loaded
  if (!window.RAG_DATA) {
    console.error('RAG_DATA bundle not found. Please ensure data.js is loaded.');
    return;
  }

  const { specs, guides, flashcards, adrs, sprints, concepts } = window.RAG_DATA;

  // State Management
  const state = {
    currentTab: 'overview',
    currentGuideIndex: 0,
    sidebarCollapsed: false,
    theme: localStorage.getItem('rag_portal_theme') || 'dark',
    // Pipeline simulator state
    pipelineMode: 'query', // 'query' | 'ingestion'
    pipelineStep: 0,
    pipelineProvider: 'ollama', // 'ollama' | 'groq'
    // Trainer flashcards state
    flashcardFilter: 'All',
    currentCardIndex: 0,
    cardFlipped: false,
    cardMastery: JSON.parse(localStorage.getItem('rag_flashcard_mastery') || '{}'),
    // Chunking playground state
    chunkSize: 1000,
    chunkOverlap: 200,
    chunkText: `The Transformer is the first transduction model relying entirely on self-attention to compute representations of its input and output without using sequence-aligned RNNs or convolution. In the following sections, we describe the Transformer, motivate self-attention and discuss its advantages over models such as recurrent neural networks. An attention function can be described as mapping a query and a set of key-value pairs to an output, where the query, keys, values, and output are all vectors. The output is computed as a weighted sum of the values, where the weight assigned to each value is computed by a compatibility function of the query with the corresponding key. In multi-head attention we found it beneficial to linearly project the queries, keys and values h times with different, learned linear projections.`,
    // Memory playground state
    memoryTurns: [
      { id: 1, query: "What is the Transformer model?", answer: "The Transformer is a neural network architecture based solely on self-attention mechanisms." },
      { id: 2, query: "Who introduced it?", answer: "Vaswani et al. introduced it in the 2017 paper 'Attention Is All You Need'." }
    ]
  };

  // Initialize theme
  document.documentElement.setAttribute('data-theme', state.theme);

  // Initialize Mermaid if available
  if (window.mermaid) {
    window.mermaid.initialize({
      startOnLoad: false,
      theme: state.theme === 'dark' ? 'dark' : 'default',
      securityLevel: 'loose'
    });
  }

  /* ==========================================================================
     Markdown Parser
     ========================================================================== */
  function parseMarkdown(md) {
    if (!md) return '';

    // Extract headings for TOC
    const headings = [];

    // Pre-process code blocks
    const codeBlocks = [];
    let processed = md.replace(/```([a-zA-Z0-9_-]*)\n([\s\S]*?)```/g, (match, lang, code) => {
      const idx = codeBlocks.length;
      if (lang === 'mermaid') {
        codeBlocks.push(`<pre class="mermaid">${escapeHtml(code.trim())}</pre>`);
      } else {
        codeBlocks.push(`<pre><code class="language-${lang || 'text'}">${escapeHtml(code.trim())}</code></pre>`);
      }
      return `%%CODEBLOCK_${idx}%%`;
    });

    // Headers with IDs
    processed = processed.replace(/^(#{1,6})\s+(.*)$/gm, (match, hashes, text) => {
      const level = hashes.length;
      const cleanText = text.trim();
      const slug = cleanText.toLowerCase().replace(/[^\w\s-]/g, '').replace(/\s+/g, '-');
      if (level === 2 || level === 3) {
        headings.push({ level, title: cleanText, slug });
      }
      return `<h${level} id="${slug}">${cleanText}</h${level}>`;
    });

    // Blockquotes
    processed = processed.replace(/^\>\s+(.*)$/gm, '<blockquote>$1</blockquote>');

    // Tables
    processed = processed.replace(/^\|(.+)\|$/gm, (match, content) => {
      const cells = content.split('|').map(c => c.trim());
      if (cells.every(c => c.match(/^:?-+:?$/))) {
        return '%%TABLE_SEP%%';
      }
      const tdType = processed.indexOf('%%TABLE_SEP%%') === -1 ? 'th' : 'td';
      return '<tr>' + cells.map(c => `<${tdType}>${c}</${tdType}>`).join('') + '</tr>';
    });

    // Wrap consecutive table rows in <table>
    processed = processed.replace(/(<tr>[\s\S]*?<\/tr>)/g, '<table>$1</table>');
    processed = processed.replace(/<\/table>\s*%%TABLE_SEP%%\s*<table>/g, '');
    processed = processed.replace(/<\/table>\s*<table>/g, '');

    // Unordered lists
    processed = processed.replace(/^\s*[-*]\s+(.*)$/gm, '<li>$1</li>');
    processed = processed.replace(/(<li>[\s\S]*?<\/li>)/g, '<ul>$1</ul>');
    processed = processed.replace(/<\/ul>\s*<ul>/g, '');

    // Bold, italic, inline code
    processed = processed.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    processed = processed.replace(/\*(.*?)\*/g, '<em>$1</em>');
    processed = processed.replace(/`([^`]+)`/g, '<code>$1</code>');

    // Links
    processed = processed.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" target="_blank" rel="noopener">$1</a>');

    // Horizontal rule
    processed = processed.replace(/^---$/gm, '<hr>');

    // Paragraphs
    const paras = processed.split(/\n\n+/).map(p => {
      p = p.trim();
      if (!p) return '';
      if (p.startsWith('<h') || p.startsWith('<pre') || p.startsWith('%%CODEBLOCK') ||
          p.startsWith('<table>') || p.startsWith('<ul>') || p.startsWith('<blockquote>') || p.startsWith('<hr>')) {
        return p;
      }
      return `<p>${p.replace(/\n/g, '<br>')}</p>`;
    });

    processed = paras.join('\n');

    // Restore code blocks
    processed = processed.replace(/%%CODEBLOCK_(\d+)%%/g, (match, idx) => codeBlocks[idx] || '');

    return { html: processed, headings };
  }

  function escapeHtml(str) {
    return str
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }

  /* ==========================================================================
     Tab Navigation
     ========================================================================== */
  const navButtons = document.querySelectorAll('.nav-tab-btn');
  const tabPanes = document.querySelectorAll('.tab-pane');

  function switchTab(tabId) {
    state.currentTab = tabId;
    navButtons.forEach(btn => {
      btn.classList.toggle('active', btn.dataset.tab === tabId);
    });
    tabPanes.forEach(pane => {
      pane.classList.toggle('active', pane.id === `tab-${tabId}`);
    });
    window.location.hash = `#${tabId}`;

    // Render tab-specific content
    if (tabId === 'guides') {
      renderActiveGuide();
    } else if (tabId === 'adrs') {
      renderAdrsList();
    } else if (tabId === 'sprints') {
      renderSprintsList();
    } else if (tabId === 'trainer') {
      renderTrainerCard();
    } else if (tabId === 'pipelines') {
      updatePipelineVisualizer();
    } else if (tabId === 'playgrounds') {
      updateChunkingPlayground();
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  navButtons.forEach(btn => {
    btn.addEventListener('click', () => switchTab(btn.dataset.tab));
  });

  /* ==========================================================================
     Tab 1: Overview & Clean Architecture
     ========================================================================== */
  const archCards = document.querySelectorAll('.arch-layer-card');
  archCards.forEach(card => {
    card.addEventListener('click', () => {
      const wasActive = card.classList.contains('active');
      archCards.forEach(c => c.classList.remove('active'));
      if (!wasActive) card.classList.add('active');
    });
  });

  /* ==========================================================================
     Tab 3: 16 Master Guides Reader & Dual Navigation
     ========================================================================== */
  const sidebarNavList = document.getElementById('sidebarGuidesList');
  const sidebarSearchInput = document.getElementById('sidebarSearchInput');
  const toggleSidebarBtn = document.getElementById('toggleSidebarBtn');
  const guidesSidebar = document.getElementById('guidesSidebar');
  const readerBody = document.getElementById('readerBody');
  const guideTitleEl = document.getElementById('guideTitle');
  const guideBreadcrumbEl = document.getElementById('guideBreadcrumb');
  const metaPillsEl = document.getElementById('guideMetaPills');
  const inpageTocEl = document.getElementById('inpageToc');
  const prevGuideBtn = document.getElementById('prevGuideBtn');
  const nextGuideBtn = document.getElementById('nextGuideBtn');

  // Render Guides Sidebar
  function renderGuidesSidebar(filterText = '') {
    sidebarNavList.innerHTML = '';
    const phases = {};

    guides.forEach((guide, idx) => {
      if (filterText && !guide.title.toLowerCase().includes(filterText.toLowerCase()) && !guide.tag.toLowerCase().includes(filterText.toLowerCase())) {
        return;
      }
      if (!phases[guide.phase]) phases[guide.phase] = [];
      phases[guide.phase].push({ guide, idx });
    });

    Object.keys(phases).forEach(phaseName => {
      const phaseGroup = document.createElement('div');
      phaseGroup.className = 'phase-group';

      const phaseTitle = document.createElement('div');
      phaseTitle.className = 'phase-title';
      phaseTitle.innerHTML = `<span>${phaseName}</span><span>(${phases[phaseName].length})</span>`;
      phaseGroup.appendChild(phaseTitle);

      phases[phaseName].forEach(({ guide, idx }) => {
        const item = document.createElement('div');
        item.className = `guide-nav-item ${idx === state.currentGuideIndex ? 'active' : ''}`;
        item.innerHTML = `
          <span class="guide-num">${guide.number}</span>
          <span>${guide.icon}</span>
          <span class="guide-title-text">${guide.title.replace(/^\d+\s*-\s*/, '')}</span>
          <span class="guide-tag-badge">${guide.tag}</span>
        `;
        item.addEventListener('click', () => {
          state.currentGuideIndex = idx;
          renderActiveGuide();
          renderGuidesSidebar(sidebarSearchInput.value);
          closeMobileSidebar();
        });
        phaseGroup.appendChild(item);
      });

      sidebarNavList.appendChild(phaseGroup);
    });
  }

  function renderActiveGuide() {
    const guide = guides[state.currentGuideIndex];
    if (!guide) return;

    guideTitleEl.textContent = guide.title;
    guideBreadcrumbEl.textContent = `${guide.phase} / ${guide.number} - ${guide.tag}`;
    metaPillsEl.innerHTML = `
      <span class="meta-pill">⏱️ ${guide.readTime}</span>
      <span class="meta-pill">🏷️ ${guide.tag}</span>
      <span class="meta-pill">📄 ${guide.file}</span>
      <span class="meta-pill">⚡ .NET 10 Clean Architecture</span>
    `;

    // Parse and render markdown
    const { html, headings } = parseMarkdown(guide.content);
    readerBody.innerHTML = html;

    // Render In-page TOC
    inpageTocEl.innerHTML = '<h4>In This Guide</h4>';
    if (headings.length > 0) {
      headings.forEach(h => {
        const link = document.createElement('a');
        link.className = `toc-link ${h.level === 3 ? 'h3-link' : ''}`;
        link.href = `#${h.slug}`;
        link.textContent = h.title;
        link.addEventListener('click', (e) => {
          e.preventDefault();
          const target = document.getElementById(h.slug);
          if (target) {
            target.scrollIntoView({ behavior: 'smooth' });
          }
        });
        inpageTocEl.appendChild(link);
      });
    } else {
      inpageTocEl.innerHTML += '<p style="font-size:12px;color:var(--text-muted);">No subheadings.</p>';
    }

    // Update Pagination buttons
    prevGuideBtn.disabled = state.currentGuideIndex === 0;
    nextGuideBtn.disabled = state.currentGuideIndex === guides.length - 1;

    // Render Mermaid diagrams
    if (window.mermaid) {
      setTimeout(() => {
        try {
          window.mermaid.run({ querySelector: '.mermaid' });
        } catch (err) {
          console.warn('Mermaid rendering notice:', err);
        }
      }, 50);
    }
  }

  const sidebarBackdrop = document.getElementById('sidebarBackdrop');

  function closeMobileSidebar() {
    if (window.innerWidth <= 900) {
      guidesSidebar.classList.remove('mobile-open');
      if (sidebarBackdrop) sidebarBackdrop.classList.remove('active');
    }
  }

  function openMobileSidebar() {
    guidesSidebar.classList.add('mobile-open');
    if (sidebarBackdrop) sidebarBackdrop.classList.add('active');
  }

  // Sidebar toggle
  toggleSidebarBtn.addEventListener('click', () => {
    if (window.innerWidth <= 900) {
      if (guidesSidebar.classList.contains('mobile-open')) {
        closeMobileSidebar();
      } else {
        openMobileSidebar();
      }
    } else {
      state.sidebarCollapsed = !state.sidebarCollapsed;
      guidesSidebar.classList.toggle('collapsed', state.sidebarCollapsed);
      toggleSidebarBtn.innerHTML = state.sidebarCollapsed ? '▶' : '◀';
    }
  });

  if (sidebarBackdrop) {
    sidebarBackdrop.addEventListener('click', closeMobileSidebar);
  }

  sidebarSearchInput.addEventListener('input', (e) => {
    renderGuidesSidebar(e.target.value);
  });

  prevGuideBtn.addEventListener('click', () => {
    if (state.currentGuideIndex > 0) {
      state.currentGuideIndex--;
      renderActiveGuide();
      renderGuidesSidebar(sidebarSearchInput.value);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  });

  nextGuideBtn.addEventListener('click', () => {
    if (state.currentGuideIndex < guides.length - 1) {
      state.currentGuideIndex++;
      renderActiveGuide();
      renderGuidesSidebar(sidebarSearchInput.value);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  });

  /* ==========================================================================
     Tab 2: Pipeline Simulator & Flow Visualizer
     ========================================================================== */
  const pipelineStepsData = {
    ingestion: [
      {
        id: 'pdf',
        name: 'PDF Documents',
        sub: 'Raw Storage',
        icon: '📄',
        desc: 'Enterprise documents (e.g. AttentionIsAllYouNeed.pdf, Bert.pdf) placed in data directory.',
        payload: { file: "AttentionIsAllYouNeed.pdf", size: "2.1 MB", pages: 15 }
      },
      {
        id: 'extract',
        name: 'PdfPig Extraction',
        sub: 'PdfDocumentExtractor',
        icon: '📑',
        desc: 'Extracts words and character geometry, normalizes whitespace and repairs line-break hyphens.',
        payload: { totalWords: 9142, extractedPages: 15, normalizationApplied: true }
      },
      {
        id: 'chunk',
        name: 'Character Chunking',
        sub: '1000 size / 200 overlap',
        icon: '✂️',
        desc: 'CharacterChunkingStrategy divides document into 1000-character segments with 200-char sliding overlap to preserve sentence boundaries.',
        payload: { totalChunks: 629, strategy: "CharacterChunkingStrategy", avgChunkSize: "982 chars" }
      },
      {
        id: 'embed',
        name: 'nomic-embed-text',
        sub: '768-dim Vector',
        icon: '🔢',
        desc: 'Local Ollama pipeline generates 768-dimensional dense float vector embeddings with SemaphoreSlim(8) concurrency throttle.',
        payload: { model: "nomic-embed-text", dimensions: 768, sampleVector: [-0.0421, 0.0891, -0.0152, "... +765 dimensions"] }
      },
      {
        id: 'qdrant',
        name: 'Qdrant Vector DB',
        sub: 'Cosine Index',
        icon: '🗄️',
        desc: 'Batch upsert vectors and payloads into `ragdemo-documents` collection with Cosine similarity index.',
        payload: { collection: "ragdemo-documents", status: "Upserted 629 points", distanceMetric: "Cosine" }
      }
    ],
    query: [
      {
        id: 'user_q',
        name: 'User Query',
        sub: 'HTTP Minimal API',
        icon: '💬',
        desc: 'User sends query to `/chat/stream` endpoint with optional conversationId.',
        payload: { query: "What is the attention mechanism?", conversationId: "conv-991a" }
      },
      {
        id: 'classify',
        name: 'Query Classifier',
        sub: 'Intent Detection',
        icon: '🎯',
        desc: 'QueryClassifier detects if input is greeting, capability question, or requires deep vector retrieval.',
        payload: { intent: "DocumentRetrieval", requiresVectorSearch: true }
      },
      {
        id: 'memory',
        name: 'Conversation Memory',
        sub: '4-Turn FIFO Rolling',
        icon: '🧠',
        desc: 'InMemoryConversationMemory loads recent turns to provide conversation context enrichment.',
        payload: { turnsLoaded: 2, queryEnriched: "What is the attention mechanism in Transformer models?" }
      },
      {
        id: 'query_embed',
        name: 'Query Embedding',
        sub: 'nomic-embed-text',
        icon: '🔢',
        desc: 'Ollama generates 768-dim vector for enriched query.',
        payload: { latencyMs: 18, vectorDims: 768 }
      },
      {
        id: 'search',
        name: 'Qdrant Cosine Match',
        sub: 'Top-K Retrieval',
        icon: '🔍',
        desc: 'Executes cosine similarity search against Qdrant, retrieving top K=3 document chunks.',
        payload: {
          topMatches: [
            { score: 0.884, source: "AttentionIsAllYouNeed.pdf", chunkIndex: 14 },
            { score: 0.841, source: "AttentionIsAllYouNeed.pdf", chunkIndex: 15 },
            { score: 0.792, source: "AttentionIsAllYouNeed.pdf", chunkIndex: 18 }
          ]
        }
      },
      {
        id: 'prompt',
        name: 'RagPromptBuilder',
        sub: 'Grounded Contract',
        icon: '✍️',
        desc: 'Injects retrieved chunks into system prompt with strict grounding constraints forbidding hallucinations.',
        payload: {
          systemInstruction: "Answer strictly based on retrieved documents. If not found, say 'I could not find the answer in the provided documents.'",
          retrievedChunksCount: 3
        }
      },
      {
        id: 'llm',
        name: 'LLM Inference',
        sub: 'Ollama vs Groq',
        icon: '🤖',
        desc: 'Model generates token stream. Local: Gemma 2 2B (~35 t/s); Cloud: Groq Llama 3.3 (~280 t/s).',
        payload: { provider: "Ollama (Gemma 2 2B)", tokensGenerated: 84, timeToFirstTokenMs: 82 }
      },
      {
        id: 'sse',
        name: 'SSE Token Stream',
        sub: 'Server-Sent Events',
        icon: '⚡',
        desc: 'Asynchronously streams tokens to client via HTTP Chunked SSE `data: {}` stream until `[DONE]`.',
        payload: { sseEventsEmitted: 42, streamComplete: true }
      }
    ]
  };

  const trackContainer = document.getElementById('pipelineNodesTrack');
  const simStepDesc = document.getElementById('simStepDesc');
  const simPayloadPreview = document.getElementById('simPayloadPreview');
  const simStepPrevBtn = document.getElementById('simStepPrevBtn');
  const simStepNextBtn = document.getElementById('simStepNextBtn');
  const simStepResetBtn = document.getElementById('simStepResetBtn');
  const modeQueryBtn = document.getElementById('modeQueryBtn');
  const modeIngestionBtn = document.getElementById('modeIngestionBtn');
  const simProviderOllamaBtn = document.getElementById('simProviderOllama');
  const simProviderGroqBtn = document.getElementById('simProviderGroq');

  function updatePipelineVisualizer() {
    const steps = pipelineStepsData[state.pipelineMode];
    trackContainer.innerHTML = '';

    steps.forEach((step, idx) => {
      const node = document.createElement('div');
      const isCompleted = idx < state.pipelineStep;
      const isActive = idx === state.pipelineStep;
      node.className = `pipeline-node ${isActive ? 'active' : ''} ${isCompleted ? 'completed' : ''}`;
      node.innerHTML = `
        <div class="node-icon">${step.icon}</div>
        <div class="node-name">${step.name}</div>
        <div class="node-sub">${step.sub}</div>
      `;
      node.addEventListener('click', () => {
        state.pipelineStep = idx;
        updatePipelineVisualizer();
      });
      trackContainer.appendChild(node);
    });

    const currentStep = steps[state.pipelineStep] || steps[0];
    simStepDesc.textContent = currentStep.desc;
    
    // Adjust provider in payload if in query mode
    const payloadCopy = JSON.parse(JSON.stringify(currentStep.payload));
    if (state.pipelineMode === 'query' && currentStep.id === 'llm') {
      if (state.pipelineProvider === 'groq') {
        payloadCopy.provider = "Groq Cloud (Llama 3.3 70B)";
        payloadCopy.timeToFirstTokenMs = 12;
        payloadCopy.tokenRate = "285 tokens/sec";
      } else {
        payloadCopy.provider = "Ollama Local (Gemma 2 2B)";
        payloadCopy.timeToFirstTokenMs = 85;
        payloadCopy.tokenRate = "38 tokens/sec";
      }
    }

    simPayloadPreview.textContent = JSON.stringify(payloadCopy, null, 2);

    simStepPrevBtn.disabled = state.pipelineStep === 0;
    simStepNextBtn.disabled = state.pipelineStep === steps.length - 1;
  }

  modeQueryBtn.addEventListener('click', () => {
    state.pipelineMode = 'query';
    state.pipelineStep = 0;
    modeQueryBtn.classList.add('active');
    modeIngestionBtn.classList.remove('active');
    updatePipelineVisualizer();
  });

  modeIngestionBtn.addEventListener('click', () => {
    state.pipelineMode = 'ingestion';
    state.pipelineStep = 0;
    modeIngestionBtn.classList.add('active');
    modeQueryBtn.classList.remove('active');
    updatePipelineVisualizer();
  });

  simProviderOllamaBtn.addEventListener('click', () => {
    state.pipelineProvider = 'ollama';
    simProviderOllamaBtn.classList.add('active');
    simProviderGroqBtn.classList.remove('active');
    updatePipelineVisualizer();
  });

  simProviderGroqBtn.addEventListener('click', () => {
    state.pipelineProvider = 'groq';
    simProviderGroqBtn.classList.add('active');
    simProviderOllamaBtn.classList.remove('active');
    updatePipelineVisualizer();
  });

  simStepPrevBtn.addEventListener('click', () => {
    if (state.pipelineStep > 0) {
      state.pipelineStep--;
      updatePipelineVisualizer();
    }
  });

  simStepNextBtn.addEventListener('click', () => {
    const steps = pipelineStepsData[state.pipelineMode];
    if (state.pipelineStep < steps.length - 1) {
      state.pipelineStep++;
      updatePipelineVisualizer();
    }
  });

  simStepResetBtn.addEventListener('click', () => {
    state.pipelineStep = 0;
    updatePipelineVisualizer();
  });

  /* ==========================================================================
     Tab 6: Interactive Mock Assessment Trainer (Flashcards)
     ========================================================================== */
  const flashcardStage = document.getElementById('flashcardStage');
  const flashcardInner = document.getElementById('flashcardInner');
  const cardTopicTag = document.getElementById('cardTopicTag');
  const cardQuestion = document.getElementById('cardQuestion');
  const cardAnswer = document.getElementById('cardAnswer');
  const cardCounter = document.getElementById('cardCounter');
  const trainerProgressBar = document.getElementById('trainerProgressBar');
  const prevCardBtn = document.getElementById('prevCardBtn');
  const nextCardBtn = document.getElementById('nextCardBtn');
  const masterCardBtn = document.getElementById('masterCardBtn');
  const reviewCardBtn = document.getElementById('reviewCardBtn');
  const categoryFiltersContainer = document.getElementById('trainerCategoryFilters');

  function getFilteredFlashcards() {
    if (state.flashcardFilter === 'All') return flashcards;
    return flashcards.filter(c => c.category.toLowerCase().includes(state.flashcardFilter.toLowerCase()));
  }

  function renderCategoryFilters() {
    const categories = ['All', 'Executive', 'Architecture', 'Retrieval', 'Chunking', 'Memory', 'Evaluation', 'Security', 'Operations'];
    categoryFiltersContainer.innerHTML = '';
    categories.forEach(cat => {
      const btn = document.createElement('button');
      btn.className = `cat-btn ${state.flashcardFilter === cat ? 'active' : ''}`;
      btn.textContent = cat;
      btn.addEventListener('click', () => {
        state.flashcardFilter = cat;
        state.currentCardIndex = 0;
        state.cardFlipped = false;
        renderCategoryFilters();
        renderTrainerCard();
      });
      categoryFiltersContainer.appendChild(btn);
    });
  }

  function renderTrainerCard() {
    const cards = getFilteredFlashcards();
    if (cards.length === 0) {
      cardTopicTag.textContent = 'Notice';
      cardQuestion.textContent = 'No cards found for this category.';
      cardAnswer.textContent = '';
      cardCounter.textContent = '0 / 0';
      return;
    }

    const card = cards[state.currentCardIndex];
    cardTopicTag.textContent = `${card.category} • Card #${state.currentCardIndex + 1}`;
    cardQuestion.textContent = card.question;
    cardAnswer.textContent = card.answer;

    // Flip card reset
    flashcardInner.classList.toggle('flipped', state.cardFlipped);

    cardCounter.textContent = `${state.currentCardIndex + 1} of ${cards.length}`;

    // Update progress bar
    const masteredCount = cards.filter(c => state.cardMastery[c.id] === 'mastered').length;
    const pct = cards.length ? Math.round((masteredCount / cards.length) * 100) : 0;
    trainerProgressBar.style.width = `${pct}%`;
  }

  flashcardStage.addEventListener('click', () => {
    state.cardFlipped = !state.cardFlipped;
    flashcardInner.classList.toggle('flipped', state.cardFlipped);
  });

  prevCardBtn.addEventListener('click', () => {
    const cards = getFilteredFlashcards();
    if (state.currentCardIndex > 0) {
      state.currentCardIndex--;
      state.cardFlipped = false;
      renderTrainerCard();
    }
  });

  nextCardBtn.addEventListener('click', () => {
    const cards = getFilteredFlashcards();
    if (state.currentCardIndex < cards.length - 1) {
      state.currentCardIndex++;
      state.cardFlipped = false;
      renderTrainerCard();
    }
  });

  masterCardBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    const cards = getFilteredFlashcards();
    const card = cards[state.currentCardIndex];
    if (card) {
      state.cardMastery[card.id] = 'mastered';
      localStorage.setItem('rag_flashcard_mastery', JSON.stringify(state.cardMastery));
      // Advance to next
      if (state.currentCardIndex < cards.length - 1) {
        state.currentCardIndex++;
      }
      state.cardFlipped = false;
      renderTrainerCard();
    }
  });

  reviewCardBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    const cards = getFilteredFlashcards();
    const card = cards[state.currentCardIndex];
    if (card) {
      state.cardMastery[card.id] = 'review';
      localStorage.setItem('rag_flashcard_mastery', JSON.stringify(state.cardMastery));
      if (state.currentCardIndex < cards.length - 1) {
        state.currentCardIndex++;
      }
      state.cardFlipped = false;
      renderTrainerCard();
    }
  });

  /* ==========================================================================
     Tab 7: Playgrounds & Interactive Tools
     ========================================================================== */
  const chunkSizeSlider = document.getElementById('chunkSizeSlider');
  const chunkOverlapSlider = document.getElementById('chunkOverlapSlider');
  const chunkSizeVal = document.getElementById('chunkSizeVal');
  const chunkOverlapVal = document.getElementById('chunkOverlapVal');
  const chunkingDisplay = document.getElementById('chunkingDisplay');
  const chunkStats = document.getElementById('chunkStats');

  function updateChunkingPlayground() {
    if (!chunkSizeSlider) return;
    const size = parseInt(chunkSizeSlider.value, 10);
    const overlap = parseInt(chunkOverlapSlider.value, 10);
    chunkSizeVal.textContent = `${size} chars`;
    chunkOverlapVal.textContent = `${overlap} chars`;

    const text = state.chunkText;
    const chunks = [];
    let start = 0;

    while (start < text.length) {
      const end = Math.min(start + size, text.length);
      const chunkStr = text.substring(start, end);
      chunks.push({
        index: chunks.length + 1,
        start,
        end,
        text: chunkStr,
        hasOverlap: start > 0 && overlap > 0
      });

      if (end >= text.length) break;
      start += (size - overlap);
      if (start <= 0 || size <= overlap) break; // guard against infinite loop
    }

    chunkStats.textContent = `Generated ${chunks.length} chunks from ${text.length} characters (Overlap: ${Math.round((overlap / size) * 100)}%)`;

    chunkingDisplay.innerHTML = '';
    chunks.forEach(c => {
      const chip = document.createElement('div');
      chip.className = 'chunk-chip';
      chip.innerHTML = `<strong>Chunk #${c.index}</strong> [Chars ${c.start}–${c.end}]<br>${c.text}`;
      chunkingDisplay.appendChild(chip);
    });
  }

  if (chunkSizeSlider) {
    chunkSizeSlider.addEventListener('input', updateChunkingPlayground);
    chunkOverlapSlider.addEventListener('input', updateChunkingPlayground);
  }

  // Memory Playground
  const addMemoryTurnBtn = document.getElementById('addMemoryTurnBtn');
  const clearMemoryBtn = document.getElementById('clearMemoryBtn');
  const memoryTurnsContainer = document.getElementById('memoryTurnsContainer');
  const memoryEvictionAlert = document.getElementById('memoryEvictionAlert');
  const enrichedQueryDisplay = document.getElementById('enrichedQueryDisplay');

  const sampleQuestions = [
    { q: "What is self-attention?", a: "Self-attention relates different positions of a single sequence." },
    { q: "How many layers in BERT base?", a: "BERT base uses 12 Transformer encoder layers." },
    { q: "What is Qdrant?", a: "Qdrant is a high-performance vector database with cosine search." },
    { q: "How does SSE work in .NET?", a: "SSE streams events using HTTP chunked transfer and IAsyncEnumerable." }
  ];

  function updateMemoryPlayground() {
    if (!memoryTurnsContainer) return;
    memoryTurnsContainer.innerHTML = '';

    state.memoryTurns.forEach((turn, idx) => {
      const card = document.createElement('div');
      card.className = 'chunk-chip';
      card.innerHTML = `
        <div style="display:flex;justify-content:space-between;font-size:11px;color:var(--accent-cyan);margin-bottom:4px;">
          <span>Turn #${turn.id}</span>
          <span>Window Slot ${idx + 1}/4</span>
        </div>
        <strong>User:</strong> ${turn.query}<br>
        <strong>Assistant:</strong> ${turn.answer}
      `;
      memoryTurnsContainer.appendChild(card);
    });

    if (state.memoryTurns.length > 0) {
      const lastQ = state.memoryTurns[state.memoryTurns.length - 1].query;
      enrichedQueryDisplay.textContent = `Enriched Vector Retrieval Query: "${lastQ} [Context: ${state.memoryTurns.map(t => t.query).slice(-3).join(' | ')}]"`;
    } else {
      enrichedQueryDisplay.textContent = 'Memory empty. Add a conversation turn.';
    }
  }

  if (addMemoryTurnBtn) {
    addMemoryTurnBtn.addEventListener('click', () => {
      const sample = sampleQuestions[Math.floor(Math.random() * sampleQuestions.length)];
      const nextId = (state.memoryTurns.length ? state.memoryTurns[state.memoryTurns.length - 1].id : 0) + 1;

      if (state.memoryTurns.length >= 4) {
        const evicted = state.memoryTurns.shift();
        memoryEvictionAlert.textContent = `⚠️ FIFO Eviction: Turn #${evicted.id} pushed out of 4-turn rolling window.`;
        memoryEvictionAlert.style.display = 'block';
        setTimeout(() => { memoryEvictionAlert.style.display = 'none'; }, 4000);
      }

      state.memoryTurns.push({ id: nextId, query: sample.q, answer: sample.a });
      updateMemoryPlayground();
    });

    clearMemoryBtn.addEventListener('click', () => {
      state.memoryTurns = [];
      updateMemoryPlayground();
    });
  }

  /* ==========================================================================
     Tab 4 & 5: ADRs & Sprints Vault
     ========================================================================== */
  function renderAdrsList() {
    const container = document.getElementById('adrsGrid');
    if (!container) return;
    container.innerHTML = '';

    adrs.forEach(adr => {
      const card = document.createElement('div');
      card.className = 'adr-card';
      card.innerHTML = `
        <div class="adr-code">${adr.code} • ${adr.status}</div>
        <div class="adr-title">${adr.title}</div>
        <div class="adr-body">${adr.context || adr.decision}</div>
      `;
      card.addEventListener('click', () => {
        // Switch to guides or show modal
        alert(`${adr.code}: ${adr.title}\n\nContext:\n${adr.context}\n\nDecision:\n${adr.decision}`);
      });
      container.appendChild(card);
    });
  }

  function renderSprintsList() {
    const container = document.getElementById('sprintsGrid');
    if (!container) return;
    container.innerHTML = '';

    sprints.forEach(sprint => {
      const card = document.createElement('div');
      card.className = 'adr-card';
      card.innerHTML = `
        <div class="adr-code">${sprint.code}</div>
        <div class="adr-title">${sprint.title}</div>
        <div class="adr-body">${sprint.goal}</div>
      `;
      container.appendChild(card);
    });
  }

  /* ==========================================================================
     Global Search Modal (Ctrl+K)
     ========================================================================== */
  const searchModalBackdrop = document.getElementById('searchModalBackdrop');
  const searchInputField = document.getElementById('searchInputField');
  const searchResultsList = document.getElementById('searchResultsList');
  const searchTriggers = document.querySelectorAll('.search-trigger');
  const closeSearchBtn = document.getElementById('closeSearchBtn');

  function openSearchModal() {
    searchModalBackdrop.classList.add('open');
    searchInputField.value = '';
    searchInputField.focus();
    renderSearchResults('');
  }

  function closeSearchModal() {
    searchModalBackdrop.classList.remove('open');
  }

  searchTriggers.forEach(btn => btn.addEventListener('click', openSearchModal));
  if (closeSearchBtn) closeSearchBtn.addEventListener('click', closeSearchModal);

  searchModalBackdrop.addEventListener('click', (e) => {
    if (e.target === searchModalBackdrop) closeSearchModal();
  });

  window.addEventListener('keydown', (e) => {
    if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
      e.preventDefault();
      if (searchModalBackdrop.classList.contains('open')) {
        closeSearchModal();
      } else {
        openSearchModal();
      }
    } else if (e.key === 'Escape' && searchModalBackdrop.classList.contains('open')) {
      closeSearchModal();
    }
  });

  function renderSearchResults(query) {
    searchResultsList.innerHTML = '';
    const q = query.toLowerCase().trim();

    const results = [];

    // Search Guides
    guides.forEach((g, idx) => {
      if (!q || g.title.toLowerCase().includes(q) || g.tag.toLowerCase().includes(q) || g.content.toLowerCase().includes(q)) {
        results.push({
          type: 'Master Guide',
          title: g.title,
          snippet: g.tag + ' • ' + g.readTime,
          action: () => {
            state.currentGuideIndex = idx;
            switchTab('guides');
            closeSearchModal();
          }
        });
      }
    });

    // Search ADRs
    adrs.forEach(a => {
      if (!q || a.code.toLowerCase().includes(q) || a.title.toLowerCase().includes(q) || a.context.toLowerCase().includes(q)) {
        results.push({
          type: 'ADR',
          title: `${a.code}: ${a.title}`,
          snippet: a.decision.substring(0, 100),
          action: () => {
            switchTab('adrs');
            closeSearchModal();
          }
        });
      }
    });

    // Search Sprints
    sprints.forEach(s => {
      if (!q || s.code.toLowerCase().includes(q) || s.title.toLowerCase().includes(q) || s.goal.toLowerCase().includes(q)) {
        results.push({
          type: 'Sprint',
          title: `${s.code}: ${s.title}`,
          snippet: s.goal.substring(0, 100),
          action: () => {
            switchTab('sprints');
            closeSearchModal();
          }
        });
      }
    });

    if (results.length === 0) {
      searchResultsList.innerHTML = '<div style="padding:16px;text-align:center;color:var(--text-muted);">No matching results found.</div>';
      return;
    }

    results.slice(0, 15).forEach(res => {
      const item = document.createElement('div');
      item.className = 'search-result-item';
      item.innerHTML = `
        <span class="result-type-badge">${res.type}</span>
        <span class="result-title">${res.title}</span>
        <span class="result-snippet">${res.snippet}</span>
      `;
      item.addEventListener('click', res.action);
      searchResultsList.appendChild(item);
    });
  }

  searchInputField.addEventListener('input', (e) => {
    renderSearchResults(e.target.value);
  });

  /* ==========================================================================
     Theme Switcher
     ========================================================================== */
  const themeToggleBtn = document.getElementById('themeToggleBtn');
  if (themeToggleBtn) {
    themeToggleBtn.addEventListener('click', () => {
      state.theme = state.theme === 'dark' ? 'light' : 'dark';
      document.documentElement.setAttribute('data-theme', state.theme);
      localStorage.setItem('rag_portal_theme', state.theme);
      themeToggleBtn.textContent = state.theme === 'dark' ? '🌙' : '☀️';

      if (window.mermaid) {
        window.mermaid.initialize({
          theme: state.theme === 'dark' ? 'dark' : 'default'
        });
        window.mermaid.run({ querySelector: '.mermaid' });
      }
    });
  }

  /* ==========================================================================
     Initial Hash Routing & Boot
     ========================================================================== */
  const initialHash = window.location.hash.replace('#', '');
  const validTabs = ['overview', 'pipelines', 'guides', 'sprints', 'adrs', 'trainer', 'playgrounds'];
  const startTab = validTabs.includes(initialHash) ? initialHash : 'overview';

  renderGuidesSidebar();
  renderCategoryFilters();
  updatePipelineVisualizer();
  updateChunkingPlayground();
  updateMemoryPlayground();
  switchTab(startTab);
});
