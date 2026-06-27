# TinyLink API

Learning project for a production-style URL shortener API using ASP.NET Core, PostgreSQL, Terraform, and AWS ECS Fargate.

## Current Status

Repository bootstrap is in progress.

## Planned Structure

```text
src/
  UrlShortener.Api
  UrlShortener.Application
  UrlShortener.Domain
  UrlShortener.Infrastructure

terraform/
  bootstrap/
  infrastructure/
```

## Commands (once solution is created)

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/UrlShortener.Api
```

## Container Smoke Test

```bash
docker build -t tinylink-api:dev .
docker run --rm -p 8080:8080 tinylink-api:dev
# In another terminal:
curl http://localhost:8080/health
```
