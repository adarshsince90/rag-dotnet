# Infrastructure

This directory contains the Docker infrastructure for the RagDemo application.

## Services

The `docker-compose.yml` defines three services:

| Service | Image | Ports | Purpose |
|---------|-------|-------|---------|
| **ollama** | `ollama/ollama:latest` | 11434 | Local LLM and embedding generation |
| **qdrant** | `qdrant/qdrant:latest` | 6333 (REST), 6334 (gRPC) | Persistent vector database |
| **rag-api** | Custom (Dockerfile) | 8080 | Containerized API (optional) |

## Quick Start

```bash
# Start Ollama and Qdrant
docker compose up -d ollama qdrant

# Pull required models
ollama pull nomic-embed-text
ollama pull gemma2:2b

# Verify
docker ps
```

## Volumes

| Volume | Mounted To | Purpose |
|--------|-----------|---------|
| `ollama-data` | `/root/.ollama` | Persists downloaded models |
| `qdrant-data` | `/qdrant/storage` | Persists vector collections |

Data survives container restarts and removals.

## Stop / Reset

```bash
# Stop services
docker compose down

# Stop and remove volumes (deletes all data)
docker compose down -v
```

## Port Reference

| Port | Service | Protocol |
|------|---------|----------|
| 11434 | Ollama API | HTTP |
| 6333 | Qdrant REST API | HTTP |
| 6334 | Qdrant gRPC API | gRPC |
| 8080 | RagDemo API (containerized) | HTTP |

## Files

| File | Purpose |
|------|---------|
| `docker-compose.yml` | Service definitions |
| `init-models.sh` | Script to pull Ollama models |
| `api/Dockerfile` | API containerization (optional) |
