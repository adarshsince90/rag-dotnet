# Streaming Responses

## Definition

Streaming delivers generated text incrementally
instead of waiting for full completion.

---

## Benefits

- Improved user experience
- Lower perceived latency
- Immediate feedback

---

## Terminology

TTFT

Time To First Token

Represents the delay before the first generated token appears.

---

## Architecture

Prompt
↓
LLM
↓
Token Stream
↓
SSE
↓
Client Rendering

---

## Transport

The system uses:

Server Sent Events (SSE)

Format:

data: token

data: token

data: token

---

## Future Enhancements

- Structured Events
- Citations
- Tool Calling Events
- Agent Progress Updates