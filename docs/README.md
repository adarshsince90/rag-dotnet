# RagDemo Documentation

## Getting Started

- [Local Development Setup](setup/Local-Development.md) — Prerequisites, infrastructure, first run
- [Infrastructure](../infrastructure/README.md) — Docker services (Ollama, Qdrant)

## Project Overview

- [Vision](00-Vision.md) — Objectives, learning philosophy, success criteria
- [Learning Roadmap](01-Learning-Roadmap.md) — AI engineering learning stages
- [Implementation Roadmap](02-Implementation-Roadmap.md) — Sprint-by-sprint build history
- [Future Directions](03-Future-Directions.md) — Planned enhancements

## Architecture

- [Architecture Overview](architecture/Architecture.md) — Layers, pipelines, component map
- [Diagnostic Metrics Catalogue](architecture/DiagnosticMetricsCatalogue.md) — All diagnostic metrics

## Architecture Decision Records

- [ADR Index](adr/README.md) — All architecture decisions with status and links

## Sprint History

| Sprint | Topic | Doc |
|--------|-------|-----|
| 00 | Foundation | [Sprint-00](sprint/Sprint-00.md) |
| 01 | Keyword Retrieval | [Sprint-01](sprint/Sprint-01.md) |
| 02 | Ranking & Diagnostics | [Sprint-02](sprint/Sprint-02.md) |
| 03 | Embeddings & Semantic Retrieval | [Sprint-03](sprint/Sprint-03.md) |
| 04 | LLM Integration | [Sprint-04](sprint/Sprint-04.md) |
| 05A | Chunking Experiments | [Sprint-05A](sprint/Sprint-05A.md) |
| 05B | PDF Support | [Sprint-05B](sprint/Sprint-05B.md) |
| 06 | Persistent Vector Storage (Qdrant) | [Sprint-06](sprint/Sprint-06.md) |
| 07A | Streaming Responses | [Sprint-07A](sprint/Sprint-07A.md) |
| 07B | Conversational Memory | [Sprint-07B](sprint/Sprint-07B.md) |
| 08 | Evaluation & Testing | [Sprint-08](sprint/Sprint-08.md) |
| 09A | Production Hardening | [Sprint-09A](sprint/Sprint-09A.md) |
| 10 | Interactive Console Client | [Sprint-10](sprint/Sprint-10.md) |
| 11 | Multi-Provider AI | [Sprint-11](sprint/Sprint-11.md) |
| 12 | Browser Chat UI | [Sprint-12](sprint/Sprint-12.md) |

## Concepts

Reference explanations of RAG concepts learned during development:

| # | Concept | Doc |
|---|---------|-----|
| 01 | Retrieval | [01-Retrieval](concepts/01-Retrieval.md) |
| 02 | Ranking | [02-Ranking](concepts/02-Ranking.md) |
| 03 | Embeddings | [03-Embeddings](concepts/03-Embeddings.md) |
| 04 | Vectors | [04-Vectors](concepts/04-Vectors.md) |
| 05 | Cosine Similarity | [05-CosineSimilarity](concepts/05-CosineSimilarity.md) |
| 06 | RAG | [06-RAG](concepts/06-RAG.md) |
| 07 | Prompting | [07-Prompting](concepts/07-Prompting.md) |
| 08 | Chunking | [08-Chunking](concepts/08-Chunking.md) |
| 09 | Vector Database | [09-vector-database](concepts/09-vector-database.md) |
| 10 | Streaming | [10-Streaming](concepts/10-Streaming.md) |
| 11 | Conversational Memory | [11-Conversational-memory](concepts/11-Conversational-memory.md) |

## Experiments

- [Chunking Lab](experiments/chunking-lab.md) — Chunking strategy experiments and findings

## Evaluation

- [Evaluation Framework](../data/evaluation/README.md) — Dataset, methodology, metrics
- [Evaluation Findings](../data/evaluation/evaluation-findings.md) — Detailed analysis
- [Evaluation Methodology](../data/evaluation/evaluation-methodology.md) — How evaluation works
