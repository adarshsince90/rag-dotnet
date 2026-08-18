#!/bin/bash

set -e

echo "Starting infrastructure..."
docker compose up -d

echo "Waiting for Ollama..."
sleep 10

echo "Pulling embedding model..."
docker exec ollama ollama pull nomic-embed-text

echo "Pulling chat model..."
docker exec ollama ollama pull phi3

echo "Installed models:"
docker exec ollama ollama list

echo "Infrastructure ready."