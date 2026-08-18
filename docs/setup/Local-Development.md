# Local Development Setup

## Start Infrastructure

cd infrastructure

./init-models.sh

## Verify Ollama

http://localhost:11434

## Verify Models

docker exec ollama ollama list

Expected:

- phi3
- nomic-embed-text

## Run API

dotnet run --project src/RagDemo.Api