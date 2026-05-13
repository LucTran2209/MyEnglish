# MyEnglish - Project Overview

## Introduction

MyEnglish is an English learning platform organized as a full-stack solution:

- `client`: React frontend application.
- `server`: ASP.NET Core backend solution.
- `wiki`: project documentation that stays outside both runtime applications.

## Project Structure

```text
MyEnglish/
|-- client/
|   `-- MyEnglish.Web/              # React + Vite frontend
|-- server/
|   |-- MyEnglish.Api/              # API layer
|   |-- MyEnglish.Application/      # Application layer
|   |-- MyEnglish.Domain/           # Domain layer
|   |-- MyEnglish.Infrastructure/   # Infrastructure services
|   `-- MyEnglish.Persistence/      # Database and repositories
|-- wiki/                           # System documentation
|-- Directory.Packages.props        # Centralized NuGet package versions
`-- MyEnglish.sln                   # Visual Studio solution
```

## Backend Summary

The backend follows a layered architecture:

- `MyEnglish.Api`: HTTP endpoints, Swagger, middleware, logging pipeline.
- `MyEnglish.Application`: commands, queries, handlers, validators, DTOs.
- `MyEnglish.Domain`: entities, value objects, repository contracts.
- `MyEnglish.Infrastructure`: JWT and cross-cutting service implementations.
- `MyEnglish.Persistence`: EF Core DbContext, migrations, entity configuration, repositories.

## Frontend Summary

The frontend is a Vite React app:

- React renders the client UI.
- TypeScript keeps frontend code type-safe.
- Vite provides local development, bundling, and production build.

## Documentation Map

- [02-Getting-Started.md](02-Getting-Started.md): setup and run instructions.
- [03-API-Endpoints.md](03-API-Endpoints.md): API endpoint documentation.
- [04-Development-Guidelines.md](04-Development-Guidelines.md): coding conventions and workflow.
- [05-Logging.md](05-Logging.md): logging design and usage.
- [06-Authentication.md](06-Authentication.md): JWT and refresh token flow.
- [07-Database-Schema.md](07-Database-Schema.md): database schema and EF Core migrations.
- [08-Technologies.md](08-Technologies.md): technologies and packages used by the system.
- [09-Architecture-Patterns.md](09-Architecture-Patterns.md): architecture and design patterns.
- [10-Engineering-Techniques.md](10-Engineering-Techniques.md): technical practices and implementation techniques.

