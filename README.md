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

## Local Database Initialization

```bash
docker compose up -d postgres
dotnet tool restore
dotnet tool run dotnet-ef database update \
  --project src/UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj \
  --startup-project src/UrlShortener.Api/UrlShortener.Api.csproj
```

PostgreSQL is exposed locally on port `55432` to avoid clashing with an existing local PostgreSQL instance.

## Container Smoke Test

```bash
docker build -t tinylink-api:dev .
docker run --rm -p 8080:8080 tinylink-api:dev
# In another terminal:
curl http://localhost:8080/health
```
