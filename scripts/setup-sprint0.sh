#!/bin/bash

set -e

echo "Creating solution structure..."

mkdir -p src
mkdir -p tests
mkdir -p docs
mkdir -p data
mkdir -p scripts

dotnet new sln -n RagDemo

echo "Creating projects..."

dotnet new webapi -n RagDemo.Api -o src/RagDemo.Api

dotnet new classlib -n RagDemo.Application -o src/RagDemo.Application

dotnet new classlib -n RagDemo.Domain -o src/RagDemo.Domain

dotnet new classlib -n RagDemo.Infrastructure -o src/RagDemo.Infrastructure

dotnet new xunit -n RagDemo.Tests -o tests/RagDemo.Tests

echo "Adding projects to solution..."

dotnet sln add src/RagDemo.Api/RagDemo.Api.csproj

dotnet sln add src/RagDemo.Application/RagDemo.Application.csproj

dotnet sln add src/RagDemo.Domain/RagDemo.Domain.csproj

dotnet sln add src/RagDemo.Infrastructure/RagDemo.Infrastructure.csproj

dotnet sln add tests/RagDemo.Tests/RagDemo.Tests.csproj

echo "Adding project references..."

dotnet add src/RagDemo.Application/RagDemo.Application.csproj reference src/RagDemo.Domain/RagDemo.Domain.csproj

dotnet add src/RagDemo.Infrastructure/RagDemo.Infrastructure.csproj reference src/RagDemo.Domain/RagDemo.Domain.csproj

dotnet add src/RagDemo.Api/RagDemo.Api.csproj reference src/RagDemo.Application/RagDemo.Application.csproj

dotnet add src/RagDemo.Api/RagDemo.Api.csproj reference src/RagDemo.Infrastructure/RagDemo.Infrastructure.csproj

dotnet add tests/RagDemo.Tests/RagDemo.Tests.csproj reference src/RagDemo.Application/RagDemo.Application.csproj

dotnet add tests/RagDemo.Tests/RagDemo.Tests.csproj reference src/RagDemo.Domain/RagDemo.Domain.csproj

echo "Creating folders..."

mkdir -p \
  data/raw \
  data/processed \
  data/chunks \
  data/embeddings

mkdir -p \
  docs/architecture \
  docs/concepts \
  docs/sprints \
  docs/adr

touch docs/00-Vision.md
touch docs/01-Learning-Roadmap.md
touch docs/02-Implementation-Roadmap.md
touch docs/architecture/Architecture.md
touch docs/adr/ADR-001-Dependency-Inversion.md
touch docs/sprints/Sprint-00.md

echo "Removing default class files..."

rm -f src/RagDemo.Application/Class1.cs
rm -f src/RagDemo.Domain/Class1.cs
rm -f src/RagDemo.Infrastructure/Class1.cs

echo "Done."