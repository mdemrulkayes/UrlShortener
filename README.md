# Url Shortener

A full-stack URL shortener built with **.NET 10**, **EF Core**, and **PostgreSQL**, with a **Minimal API** backend and a **Blazor Server** frontend.

## Project Structure

- [`src/API/UrlShortener.Api`](src/API/UrlShortener.Api) – Minimal API, database access, migrations, business logic.
- [`src/WEB/UrlShortener`](src/WEB/UrlShortener) – Blazor Server UI for creating and listing shortened URLs.
- [`tests/UrlShortener.UnitTest`](tests/UrlShortener.UnitTest) – unit tests for service logic and utilities.
- [`docker-compose.yml`](docker-compose.yml) – container orchestration for local development.

## Technology Stack

### Backend

- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL (`Npgsql`)
- FluentValidation
- OpenAPI + Scalar UI

### Frontend

- Blazor Server (.NET 10)
- MudBlazor
- Refit HTTP client

### Testing

- xUnit
- Shouldly
- EF Core InMemory provider

### DevOps / Tooling

- Docker & Docker Compose
- GitHub Actions CI ([`.github/workflows/ci.yml`](.github/workflows/ci.yml))

## Core Features

- Create short code from long URL
- Redirect from short code to original URL
- List all shortened URLs (ordered newest first)
- URL request validation
- DB schema managed by EF Core migrations

## Run Locally

### Option 1: Docker Compose

```bash
docker compose up --build
```

### Option 2: .NET CLI

Run API:

```bash
dotnet run --project src/API/UrlShortener.Api/UrlShortener.Api.csproj
```

Run Web:

```bash
dotnet run --project src/WEB/UrlShortener/UrlShortener.csproj
```

## Run Tests

```bash
dotnet test tests/UrlShortener.UnitTest/UrlShortener.UnitTest.csproj
```

## AI Assistance

Parts of this project were developed and refined with assistance from AI tools (including code suggestions, refactoring support, and documentation drafting), with final review and decisions made by the author.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
