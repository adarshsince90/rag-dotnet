/**
 * RAG Assistant .NET 10 Studio - Client Orchestration
 * Handles SSE streaming, real-time telemetry HUD updates, markdown parsing, and session state.
 */

(() => {
  // Session State
  let conversationId = localStorage.getItem("conversation-id");
  if (!conversationId) {
    conversationId = crypto.randomUUID();
    localStorage.setItem("conversation-id", conversationId);
  }

  // Base URL (handles both http://localhost:5000 and direct file:// inspection)
  const apiBase = (window.location.protocol === "file:") ? "http://localhost:5000" : "";

  // DOM Elements
  const chatContainer = document.getElementById("chat-container");
  const questionInput = document.getElementById("question-input");
  const sendButton = document.getElementById("send-btn");
  const newChatBtn = document.getElementById("new-chat-btn");
  const conversationLabel = document.getElementById("conversationLabel");
  const apiStatusBadge = document.getElementById("apiStatusBadge");
  const apiStatusText = document.getElementById("apiStatusText");
  const themeToggleBtn = document.getElementById("themeToggleBtn");

  // Theme State
  let currentTheme = localStorage.getItem("rag_chat_theme") || "dark";
  document.documentElement.setAttribute("data-theme", currentTheme);
  if (themeToggleBtn) {
    themeToggleBtn.textContent = currentTheme === "dark" ? "🌙" : "☀️";
    themeToggleBtn.addEventListener("click", () => {
      currentTheme = currentTheme === "dark" ? "light" : "dark";
      document.documentElement.setAttribute("data-theme", currentTheme);
      localStorage.setItem("rag_chat_theme", currentTheme);
      themeToggleBtn.textContent = currentTheme === "dark" ? "🌙" : "☀️";
    });
  }

  // Telemetry HUD Elements
  const hudRetrievalMs = document.getElementById("hudRetrievalMs");
  const hudChunksPill = document.getElementById("hudChunksPill");
  const hudHighestScore = document.getElementById("hudHighestScore");
  const hudScoreBar = document.getElementById("hudScoreBar");
  const hudGenerationMs = document.getElementById("hudGenerationMs");
  const hudTotalMs = document.getElementById("hudTotalMs");
  const hudStatusPill = document.getElementById("hudStatusPill");

  // Initialize UI
  updateConversationLabel();
  checkApiHealth();
  setupTextareaAutoResize();

  // Attach Event Listeners
  if (newChatBtn) {
    newChatBtn.addEventListener("click", startNewConversation);
  }

  if (sendButton) {
    sendButton.addEventListener("click", sendMessage);
  }

  if (questionInput) {
    questionInput.addEventListener("keydown", (e) => {
      if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        sendMessage();
      }
    });

    questionInput.addEventListener("input", () => {
      setupTextareaAutoResize();
    });
  }

  // Delegate starter prompt chip clicks
  document.addEventListener("click", (e) => {
    const chip = e.target.closest(".prompt-chip");
    if (chip && chip.dataset.prompt) {
      questionInput.value = chip.dataset.prompt;
      sendMessage();
    }
  });

  /**
   * Check API Health status
   */
  async function checkApiHealth() {
    try {
      const res = await fetch(`${apiBase}/diagnostic/health`);
      if (res.ok) {
        apiStatusBadge.className = "badge-status online";
        apiStatusText.textContent = "API Online (Ollama/Groq)";
      } else {
        markApiOffline();
      }
    } catch {
      markApiOffline();
    }
  }

  function markApiOffline() {
    apiStatusBadge.className = "badge-status offline";
    apiStatusText.textContent = "API Offline (Run dotnet run)";
  }

  /**
   * Reset / Start New Conversation
   */
  function startNewConversation() {
    if (!confirm("Start a new conversation session? This resets the 4-turn sliding memory.")) {
      return;
    }

    conversationId = crypto.randomUUID();
    localStorage.setItem("conversation-id", conversationId);
    updateConversationLabel();

    // Reset HUD
    resetTelemetryHud();

    // Reset Chat Messages to Welcome state
    chatContainer.innerHTML = getWelcomeHeroMarkup();
    questionInput.value = "";
    questionInput.focus();
  }

  function updateConversationLabel() {
    if (conversationLabel) {
      conversationLabel.textContent = `Session: ${conversationId.substring(0, 8)}...${conversationId.substring(conversationId.length - 4)} • 4-Turn Window`;
    }
  }

  function resetTelemetryHud() {
    hudRetrievalMs.innerHTML = `-- <span class="hud-unit">ms</span>`;
    hudChunksPill.textContent = `0 Chunks`;
    hudHighestScore.textContent = `--`;
    hudScoreBar.style.width = `0%`;
    hudGenerationMs.innerHTML = `-- <span class="hud-unit">ms</span>`;
    hudTotalMs.innerHTML = `-- <span class="hud-unit">ms</span>`;
    hudStatusPill.textContent = `Idle`;
  }

  /**
   * Send question and consume SSE stream
   */
  async function sendMessage() {
    const question = questionInput.value.trim();
    if (!question) return;

    // Disable inputs while processing
    sendButton.disabled = true;
    questionInput.disabled = true;
    questionInput.value = "";
    setupTextareaAutoResize();

    // Remove welcome hero if present
    const hero = document.getElementById("welcomeHero");
    if (hero) hero.remove();

    // Append User Message Card
    appendUserMessage(question);

    // Create Assistant Message Card with typing dots
    const assistantCard = createAssistantCard();

    // Update HUD status to Active
    hudStatusPill.textContent = "Streaming...";

    const startTime = performance.now();

    try {
      const response = await fetch(`${apiBase}/conversation/stream`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          conversationId: conversationId,
          question: question
        })
      });

      if (!response.ok) {
        if (window.location.protocol === "file:") {
          assistantCard.answerEl.innerHTML = `<span style="color:var(--accent-amber);">⚠️ Opened via file:// protocol. Start the backend with <code>dotnet run --project src/RagDemo.Api</code> and open <a href="http://localhost:5000" target="_blank" style="color:var(--accent-cyan);text-decoration:underline;">http://localhost:5000</a> to interact with the live model.</span>`;
        } else {
          assistantCard.answerEl.innerHTML = `<span style="color:var(--accent-rose);">❌ Server returned error: ${response.status} ${response.statusText}</span>`;
        }
        finishTurn();
        return;
      }

      const reader = response.body.getReader();
      const decoder = new TextDecoder();
      let buffer = "";
      let rawAnswerText = "";
      let retrievalData = null;
      let completionData = null;

      while (true) {
        const { value, done } = await reader.read();
        if (done) break;

        buffer += decoder.decode(value, { stream: true });
        const events = buffer.split("\n\n");
        buffer = events.pop(); // keep remainder

        for (const evt of events) {
          const parsed = parseSseEvent(evt);
          if (!parsed) continue;

          switch (parsed.type) {
            case "token": {
              let tokenStr = parsed.data;
              try {
                tokenStr = JSON.parse(parsed.data);
              } catch {
                // Already raw string
              }

              // Remove initial typing indicator on first token
              if (assistantCard.typingEl) {
                assistantCard.typingEl.remove();
                assistantCard.typingEl = null;
              }

              rawAnswerText += tokenStr;
              updateStreamingAnswer(assistantCard.answerEl, rawAnswerText);
              scrollToBottom();
              break;
            }

            case "retrieval": {
              try {
                retrievalData = JSON.parse(parsed.data);
                updateRetrievalTelemetry(retrievalData);
                renderSourcesList(assistantCard.sourcesEl, retrievalData);
              } catch (e) {
                console.error("Error parsing retrieval event", e);
              }
              break;
            }

            case "completed": {
              try {
                completionData = JSON.parse(parsed.data);
                updateCompletionTelemetry(completionData);
              } catch (e) {
                console.error("Error parsing completed event", e);
              }

              // Remove streaming cursor
              const cursor = assistantCard.answerEl.querySelector(".streaming-cursor");
              if (cursor) cursor.remove();

              // Final markdown formatting
              assistantCard.answerEl.innerHTML = formatMarkdown(rawAnswerText);

              // Render Diagnostics
              renderDiagnosticsAccordion(assistantCard.diagnosticsEl, retrievalData, completionData);
              finishTurn();
              break;
            }
          }
        }
      }

      // Safeguard if completed event was missed
      if (sendButton.disabled) {
        const cursor = assistantCard.answerEl.querySelector(".streaming-cursor");
        if (cursor) cursor.remove();
        assistantCard.answerEl.innerHTML = formatMarkdown(rawAnswerText);
        finishTurn();
      }

    } catch (err) {
      console.error("Streaming error:", err);
      if (assistantCard.typingEl) assistantCard.typingEl.remove();
      assistantCard.answerEl.innerHTML = `<span style="color:var(--accent-rose);">❌ Unable to connect to RAG API. Ensure the .NET server is running at this endpoint.</span>`;
      finishTurn();
    }
  }

  function finishTurn() {
    sendButton.disabled = false;
    questionInput.disabled = false;
    questionInput.focus();
    hudStatusPill.textContent = "Idle";
    scrollToBottom();
  }

  /**
   * Telemetry Updates
   */
  function updateRetrievalTelemetry(retrieval) {
    if (!retrieval) return;
    if (retrieval.RetrievalMs !== undefined) {
      hudRetrievalMs.innerHTML = `${retrieval.RetrievalMs} <span class="hud-unit">ms</span>`;
    }
    if (retrieval.ReturnedChunks !== undefined) {
      hudChunksPill.textContent = `${retrieval.ReturnedChunks} Chunks`;
    }
    if (retrieval.HighestScore !== undefined) {
      const score = retrieval.HighestScore;
      hudHighestScore.textContent = score.toFixed(3);
      hudScoreBar.style.width = `${Math.min(100, Math.round(score * 100))}%`;
    }
  }

  function updateCompletionTelemetry(completion) {
    if (!completion) return;
    if (completion.GenerationMs !== undefined) {
      hudGenerationMs.innerHTML = `${completion.GenerationMs} <span class="hud-unit">ms</span>`;
    }
    if (completion.TotalMs !== undefined) {
      hudTotalMs.innerHTML = `${completion.TotalMs} <span class="hud-unit">ms</span>`;
    }
  }

  /**
   * UI Rendering Helpers
   */
  function appendUserMessage(text) {
    const wrapper = document.createElement("div");
    wrapper.className = "message-wrapper user";
    const now = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    wrapper.innerHTML = `
      <div class="user-card">
        <div class="msg-meta">
          <span>👤 User Query</span>
          <span class="msg-timestamp">${now}</span>
        </div>
        <div class="msg-body">${escapeHtml(text)}</div>
      </div>
    `;

    chatContainer.appendChild(wrapper);
    scrollToBottom();
  }

  function createAssistantCard() {
    const wrapper = document.createElement("div");
    wrapper.className = "message-wrapper assistant";
    const now = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    wrapper.innerHTML = `
      <div class="assistant-card">
        <div class="msg-meta">
          <div class="assistant-identity">
            <div class="assistant-avatar">🤖</div>
            <span>RAG Assistant</span>
          </div>
          <span class="msg-timestamp">${now}</span>
        </div>

        <div class="answer-content">
          <div class="typing-dots">
            <span></span><span></span><span></span>
          </div>
        </div>

        <div class="sources-slot"></div>
        <div class="diagnostics-slot"></div>
      </div>
    `;

    chatContainer.appendChild(wrapper);
    scrollToBottom();

    return {
      wrapper,
      answerEl: wrapper.querySelector(".answer-content"),
      typingEl: wrapper.querySelector(".typing-dots"),
      sourcesEl: wrapper.querySelector(".sources-slot"),
      diagnosticsEl: wrapper.querySelector(".diagnostics-slot")
    };
  }

  function updateStreamingAnswer(container, rawText) {
    let cursor = container.querySelector(".streaming-cursor");
    if (!cursor) {
      cursor = document.createElement("span");
      cursor.className = "streaming-cursor";
      cursor.textContent = "▌";
    }

    container.textContent = rawText;
    container.appendChild(cursor);
  }

  function renderSourcesList(container, retrieval) {
    if (!retrieval || !retrieval.Sources || retrieval.Sources.length === 0) return;

    let html = `
      <div class="sources-container">
        <div class="sources-header">
          <span>📚 Grounded Sources</span>
          <span>(${retrieval.Sources.length} references)</span>
        </div>
        <div class="sources-list">
    `;

    retrieval.Sources.forEach(src => {
      html += `
        <span class="source-tag">
          <span>📄</span>
          <span>${escapeHtml(src)}</span>
        </span>
      `;
    });

    html += `</div></div>`;
    container.innerHTML = html;
  }

  function renderDiagnosticsAccordion(container, retrieval, completion) {
    if (!retrieval && !completion) return;

    const retMs = retrieval ? `${retrieval.RetrievalMs} ms` : "--";
    const genMs = completion ? `${completion.GenerationMs} ms` : "--";
    const totalMs = completion ? `${completion.TotalMs} ms` : "--";
    const avgScore = retrieval && retrieval.AverageScore ? retrieval.AverageScore.toFixed(3) : "--";
    const maxScore = retrieval && retrieval.HighestScore ? retrieval.HighestScore.toFixed(3) : "--";
    const chunks = retrieval ? retrieval.ReturnedChunks : "--";

    container.innerHTML = `
      <details class="diagnostics-accordion">
        <summary>
          <span>⚡ Turn Diagnostics &amp; Telemetry Breakdown</span>
          <span>▼</span>
        </summary>
        <div class="diagnostics-panel">
          <div class="diag-item">
            <div class="label">Vector Retrieval</div>
            <div class="val">${retMs}</div>
          </div>
          <div class="diag-item">
            <div class="label">LLM Generation</div>
            <div class="val">${genMs}</div>
          </div>
          <div class="diag-item">
            <div class="label">End-to-End Latency</div>
            <div class="val">${totalMs}</div>
          </div>
          <div class="diag-item">
            <div class="label">Chunks Injected</div>
            <div class="val">${chunks}</div>
          </div>
          <div class="diag-item">
            <div class="label">Max Cosine Score</div>
            <div class="val">${maxScore}</div>
          </div>
          <div class="diag-item">
            <div class="label">Avg Cosine Score</div>
            <div class="val">${avgScore}</div>
          </div>
        </div>
      </details>
    `;
  }

  function parseSseEvent(rawEvent) {
    const lines = rawEvent.split("\n");
    const eventLine = lines.find(x => x.startsWith("event:"));
    const dataLine = lines.find(x => x.startsWith("data:"));

    if (!eventLine || !dataLine) return null;

    return {
      type: eventLine.replace("event:", "").trim(),
      data: dataLine.replace("data:", "").trim()
    };
  }

  /**
   * Lightweight Markdown Formatter
   */
  function formatMarkdown(text) {
    if (!text) return "";
    let safe = escapeHtml(text);

    // Code blocks ```lang ... ```
    safe = safe.replace(/```([a-zA-Z0-9_-]*)\n([\s\S]*?)```/g, (match, lang, code) => {
      return `<pre><code class="language-${lang || 'text'}">${code.trim()}</code></pre>`;
    });

    // Inline code `code`
    safe = safe.replace(/`([^`]+)`/g, '<code>$1</code>');

    // Bold **text**
    safe = safe.replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>');

    // Italic *text*
    safe = safe.replace(/\*([^*]+)\*/g, '<em>$1</em>');

    // Bullet points
    safe = safe.replace(/^\s*-\s+(.*)$/gm, '<li>$1</li>');
    safe = safe.replace(/(<li>.*<\/li>)/s, '<ul>$1</ul>');

    // Line breaks to paragraphs
    const paragraphs = safe.split(/\n\n+/);
    return paragraphs.map(p => {
      if (p.startsWith('<pre>') || p.startsWith('<ul>')) return p;
      return `<p>${p.replace(/\n/g, '<br>')}</p>`;
    }).join('');
  }

  function escapeHtml(str) {
    const div = document.createElement("div");
    div.textContent = str;
    return div.innerHTML;
  }

  function scrollToBottom() {
    chatContainer.scrollTop = chatContainer.scrollHeight;
  }

  function setupTextareaAutoResize() {
    if (!questionInput) return;
    questionInput.style.height = 'auto';
    questionInput.style.height = Math.min(questionInput.scrollHeight, 120) + 'px';
  }

  function getWelcomeHeroMarkup() {
    return `
      <div class="welcome-hero" id="welcomeHero">
        <div class="hero-badge">Conversational RAG • Grounded First Principles</div>
        <h2>Enterprise Knowledge Assistant</h2>
        <p class="hero-desc">
          Query the pre-indexed technical corpus (including <em>Attention Is All You Need</em>, <em>BERT Pretraining</em>, and <em>Language Models are Few-Shot Learners</em>). The assistant enforces strict boundary grounding, refusing to hallucinate when context is absent.
        </p>

        <div class="hero-specs-row">
          <div class="spec-pill"><span>🏛️</span> Clean Architecture</div>
          <div class="spec-pill"><span>🔢</span> nomic-embed-text (768 Dim)</div>
          <div class="spec-pill"><span>📦</span> Qdrant Vector Collection</div>
          <div class="spec-pill"><span>🧠</span> 4-Turn Rolling Memory</div>
          <div class="spec-pill"><span>🛡️</span> Zero Hallucination Policy</div>
        </div>

        <div class="starter-prompts-section">
          <div class="starter-header">Suggested Assessment &amp; Technical Queries:</div>
          <div class="starter-grid">
            <button class="prompt-chip" data-prompt="What is self-attention?">
              <span class="chip-icon">🔹</span>
              <span class="chip-text">What is self-attention?</span>
            </button>
            <button class="prompt-chip" data-prompt="Summarize the Transformer architecture.">
              <span class="chip-icon">🏛️</span>
              <span class="chip-text">Summarize the Transformer architecture.</span>
            </button>
            <button class="prompt-chip" data-prompt="What are the advantages of transformers over RNNs?">
              <span class="chip-icon">⚡</span>
              <span class="chip-text">What are advantages of transformers over RNNs?</span>
            </button>
            <button class="prompt-chip" data-prompt="Refusal Probe: What is the capital of France?">
              <span class="chip-icon">🛡️</span>
              <span class="chip-text">Refusal Probe: What is the capital of France?</span>
            </button>
          </div>
        </div>
      </div>
    `;
  }
})();