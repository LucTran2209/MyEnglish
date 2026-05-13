# Engineering Techniques

This file lists implementation techniques and engineering practices used in MyEnglish.

## Layered Solution Organization

The repository is organized by runtime boundary:

- `client`: frontend application.
- `server`: backend applications and libraries.
- `wiki`: system documentation.

The backend is further organized by responsibility: API, Application, Domain, Infrastructure, and Persistence.

## Centralized Package Versions

NuGet versions are managed in `Directory.Packages.props`. This avoids version drift between backend projects.

## Request Validation

FluentValidation is used to validate incoming requests and application commands before business logic runs.

## Token-Based Authentication

Authentication uses short-lived JWT access tokens and refresh tokens. This supports stateless API authentication while allowing sessions to be renewed safely.

## Password Hashing

Passwords are hashed with BCrypt instead of being stored as plain text or reversible encrypted values.

## Entity Configuration

EF Core entity mapping is separated into configuration classes. This keeps entity classes focused on domain behavior and keeps database mapping concerns in persistence.

## Soft Delete

Domain abstractions include soft delete support. EF Core query filters can hide soft-deleted records from normal queries.

## Auditing

Shared entity base classes support created/updated metadata. This makes audit fields consistent across entities.

## Async Data Access

Repository and handler methods use async APIs. This prevents blocking threads during database and IO operations.

## API Documentation

Swagger/OpenAPI is enabled for discovering and testing backend endpoints during development.

## Frontend Build Pipeline

The React client uses Vite scripts:

```powershell
npm run dev
npm run build
npm run preview
```

## Backend Build Pipeline

The server can be built from the solution root:

```powershell
dotnet build MyEnglish.sln
```

## Migration Workflow

EF Core migrations are created from the solution root:

```powershell
dotnet ef migrations add MigrationName --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
dotnet ef database update --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
```

