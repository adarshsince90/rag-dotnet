console.log("chat.js loaded");

let conversationId =
    localStorage.getItem("conversation-id");

if (!conversationId) {

    conversationId =
        crypto.randomUUID();

    localStorage.setItem(
        "conversation-id",
        conversationId);
}

document
    .getElementById("new-chat-btn")
    .addEventListener(
        "click",
        startNewConversation);

const chatContainer =
    document.getElementById("chat-container");

const sendButton =
    document.getElementById("send-btn");

const questionInput =
    document.getElementById("question-input");

sendButton.addEventListener(
    "click",
    sendMessage);

questionInput.addEventListener(
    "keydown",
    function (event) {

        if (event.key === "Enter") {

            sendMessage();
        }
    });

async function sendMessage() {

    sendButton.disabled = true;
    questionInput.disabled = true;
    
    const question =
        questionInput.value.trim();

    if (!question) {
        return;
    }

    questionInput.value = "";

    appendUserMessage(question);

    const assistantElements =
        createAssistantMessage();

    await streamQuestion(
        question,
        assistantElements.answer,
        assistantElements.sources,
        assistantElements.diagnostics);
}

function startNewConversation() {

    if (!confirm(
        "Start a new conversation?"))
    {
        return;
    }

    conversationId =
        crypto.randomUUID();

    localStorage.setItem(
        "conversation-id",
        conversationId);

    updateConversationLabel();

    chatContainer.innerHTML =
        getWelcomeMarkup();
}

function getWelcomeMarkup() {

    return `
        <div class="welcome-message">

            <h4>Welcome</h4>

            <p>
                Ask questions about your indexed documents.
            </p>

            <ul>
                <li>What is self-attention?</li>
                <li>Summarize the transformer architecture.</li>
                <li>What limitations are discussed?</li>
            </ul>

        </div>
    `;
}

function updateConversationLabel() {

    const label =
        document.getElementById(
            "conversation-label");

    label.textContent =
        `Conversation: ${conversationId}`;
}

function appendUserMessage(text) {

    removeWelcomeMessage();

    const wrapper =
        document.createElement("div");

    wrapper.className =
        "message";

    wrapper.innerHTML =
        `
        <div class="user-card">
            <strong>You</strong>
            <div class="mt-2">${escapeHtml(text)}</div>
        </div>
        `;

    chatContainer.appendChild(wrapper);

    scrollToBottom();
}

function createAssistantMessage() {

    removeWelcomeMessage();

    const wrapper =
        document.createElement("div");

    wrapper.className =
        "message";

    wrapper.innerHTML =
`
<div class="assistant-card">

    <strong>🤖 Assistant</strong>

    <div class="answer mt-2"></div>

    <div class="typing-indicator">

        <span></span>
        <span></span>
        <span></span>

    </div>

    <div class="sources"></div>

    <div class="diagnostics"></div>

</div>
`;

    chatContainer.appendChild(wrapper);

    scrollToBottom();

    return {
        answer: wrapper.querySelector(".answer"),
        sources: wrapper.querySelector(".sources"),
        diagnostics: wrapper.querySelector(".diagnostics")
    };
}

async function streamQuestion(
    question,
    answerElement,
    sourcesElement,
    diagnosticsElement) {

    let retrieval = null;
    let completion = null;

    try {

        const response =
            await fetch(
                "/conversation/stream",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        conversationId: conversationId,
                        question: question
                    })
                });

        if (!response.ok) {

            answerElement.textContent =
                "Failed to contact API.";

            return;
        }

        const reader =
            response.body.getReader();

        const decoder =
            new TextDecoder();

        let buffer = "";

        while (true) {

            const { value, done } =
                await reader.read();

            if (done)
                break;

            buffer += decoder.decode(
                value,
                { stream: true });

            const events =
                buffer.split("\n\n");

            buffer = events.pop();

            for (const evt of events) {

                const parsed =
                    parseEvent(evt);

                if (!parsed)
                    continue;

                switch (parsed.type) {

                    case "token":

                        const token =
                            JSON.parse(parsed.data);

                        appendToken(
                            answerElement,
                            token);

                        scrollToBottom();
                        break;

                    case "retrieval":

                        retrieval =
                            JSON.parse(parsed.data);

                        break;

                    case "completed":

                        completion =
                            JSON.parse(parsed.data);

                        const cursor =
                                answerElement.querySelector(
                                    ".streaming-cursor");

                            if (cursor) {
                                cursor.remove();
                            }

                        const typingIndicator =
                                answerElement.parentElement
                                    .querySelector(".typing-indicator");

                            if (typingIndicator) {
                                typingIndicator.remove();
                            }

                        renderSources(
                            sourcesElement,
                            retrieval);

                        renderDiagnostics(
                            diagnosticsElement,
                            retrieval,
                            completion);

                        questionInput.disabled = false;
                        sendButton.disabled = false;
                        questionInput.focus();

                        break;
                }
            }
        }

    }
    catch (error) {

        console.error(error);

        answerElement.textContent =
            "Unable to connect to the server.";
            
        questionInput.disabled = false;
        sendButton.disabled = false;
    }
}

function appendToken(
    answerElement,
    token)
{
    const cursor =
        answerElement.querySelector(
            ".streaming-cursor");

    if (cursor) {
        cursor.remove();
    }

    answerElement.appendChild(
        document.createTextNode(token));

    const newCursor =
        document.createElement("span");

    newCursor.className =
        "streaming-cursor";

    newCursor.textContent = "▌";

    answerElement.appendChild(
        newCursor);
}

function updateStreamingText(
    answerElement,
    token) {

    const existingCursor =
        answerElement.querySelector(
            ".streaming-cursor");

    if (existingCursor) {
        existingCursor.remove();
    }

    answerElement.appendChild(
        document.createTextNode(token));

    const cursor =
        document.createElement("span");

    cursor.className =
        "streaming-cursor";

    cursor.textContent = "▌";

    answerElement.appendChild(cursor);
}

function parseEvent(rawEvent) {

    const lines =
        rawEvent.split("\n");

    const eventLine =
        lines.find(x =>
            x.startsWith("event:"));

    const dataLine =
        lines.find(x =>
            x.startsWith("data:"));

    if (!eventLine || !dataLine)
        return null;

    return {
        type:
            eventLine.replace(
                "event:",
                "").trim(),

        data:
            dataLine.replace(
                "data:",
                "").trim()
    };
}

function renderSources(
    container,
    retrieval) {

    if (!retrieval)
        return;

    let html =
        "<hr><h6>📚 Sources</h6>";

    retrieval.Sources.forEach(source => {

        html +=
            `<span class="source-chip">
                📄 ${source}
             </span>`;
    });

    container.innerHTML = html;
}

function renderDiagnostics(
    container,
    retrieval,
    completion) {

    if (!retrieval || !completion)
        return;

    container.innerHTML =
`
<hr>

<details>

    <summary>
        Diagnostics
    </summary>

    <div class="details-panel">

        <div class="metric-grid">

            <div class="metric">
                Retrieval: ${retrieval.RetrievalMs} ms
            </div>

            <div class="metric">
                Chunks: ${retrieval.ReturnedChunks}
            </div>

            <div class="metric">
                Avg Score:
                ${retrieval.AverageScore.toFixed(3)}
            </div>

            <div class="metric">
                Max Score:
                ${retrieval.HighestScore.toFixed(3)}
            </div>

            <div class="metric">
                Generation:
                ${completion.GenerationMs} ms
            </div>

            <div class="metric">
                Total:
                ${completion.TotalMs} ms
            </div>

        </div>

    </div>

</details>
`;
}

function removeWelcomeMessage() {

    const welcome =
        document.querySelector(
            ".welcome-message");

    if (welcome) {
        welcome.remove();
    }
}

function scrollToBottom() {

    chatContainer.scrollTop =
        chatContainer.scrollHeight;
}

function escapeHtml(text) {

    const div =
        document.createElement("div");

    div.textContent = text;

    return div.innerHTML;
}

questionInput.focus();