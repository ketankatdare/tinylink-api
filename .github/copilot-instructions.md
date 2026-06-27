# Project Overview

This repository is a learning project intended to simulate a real-world cloud-native production application.

The application is a URL Shortener REST API built using ASP.NET Core and deployed to AWS.

## Goals

- Learn Amazon ECS Fargate
- Learn Amazon RDS PostgreSQL
- Learn Terraform Infrastructure as Code
- Learn GitHub Actions with GitHub OIDC authentication
- Follow production-quality architecture while keeping infrastructure cost low.

## Tech Stack

- ASP.NET Core (.NET 10)
- C#
- Entity Framework Core
- PostgreSQL
- Docker
- Amazon ECS Fargate
- Amazon ECR
- Amazon RDS PostgreSQL
- Application Load Balancer
- Terraform
- GitHub Actions

## Project Principles

- Keep the solution simple and maintainable.
- Avoid unnecessary abstractions and over-engineering.
- Prefer built-in .NET features before introducing third-party libraries.
- Follow Clean Architecture concepts without becoming overly complex.
- Use asynchronous APIs where appropriate.
- Write readable, self-documenting code.
- Follow SOLID principles pragmatically.
- Favor composition over inheritance.

## Infrastructure

Infrastructure is managed entirely using Terraform.

Terraform provisions:

- Custom VPC
- Public and Private Subnets
- Internet Gateway
- Route Tables
- Security Groups
- Amazon ECR
- Amazon ECS Cluster
- ECS Service
- Task Definition
- Application Load Balancer
- Amazon RDS PostgreSQL
- IAM Roles and Policies

Infrastructure should remain modular and easy to understand.

## CI/CD

GitHub Actions should:

1. Build the application
2. Execute tests
3. Build Docker image
4. Push image to Amazon ECR
5. Deploy to Amazon ECS

Authentication to AWS should use GitHub OIDC. Do not use long-lived AWS credentials.

## Coding Guidelines

- Use minimal APIs only when appropriate; otherwise prefer Controllers.
- Use dependency injection.
- Use configuration via Options pattern.
- Use EF Core migrations.
- Handle errors using centralized middleware.
- Implement structured logging.
- Return proper HTTP status codes.
- Validate input models.
- Keep controllers thin.
- Keep business logic inside the Application layer.
- Keep Infrastructure responsible for AWS and database concerns.

## Architecture

Prefer the following project structure:

src/
    UrlShortener.Api
    UrlShortener.Application
    UrlShortener.Domain
    UrlShortener.Infrastructure

terraform/
    bootstrap/
    infrastructure/

.github/
    workflows/

docs/

## Expectations

When suggesting code:

- Prefer production-quality implementations.
- Explain trade-offs when there are multiple approaches.
- Keep AWS infrastructure aligned with ECS Fargate best practices.
- Keep Terraform readable rather than overly optimized.
- Minimize AWS costs unless production behavior is explicitly requested.