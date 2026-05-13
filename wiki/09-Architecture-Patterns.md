# Architecture and Design Patterns

This file lists the architecture and design patterns used in MyEnglish.

## Clean Architecture

The backend is split into layers with clear responsibilities:

- **API**: receives HTTP requests and exposes endpoints.
- **Application**: coordinates use cases through commands, queries, handlers, DTOs, and validators.
- **Domain**: owns business concepts such as entities and value objects.
- **Infrastructure**: implements technical services such as JWT token generation.
- **Persistence**: implements database access using EF Core.

The dependency direction keeps business logic away from framework-heavy code where possible.

## Domain-Driven Design

The domain project contains business concepts such as:

- `User`
- `UserRefreshToken`
- value objects such as email, password, and refresh token concepts
- repository contracts

DDD is used to keep important business rules close to the model instead of scattering them across endpoints.

## CQRS

Commands and queries are separated by intent:

- **Commands** change system state, such as register, login, logout, refresh token, or update profile.
- **Queries** read system state, such as retrieving a user by id.

MediatR is used to dispatch requests to their handlers.

## Repository Pattern

Repository interfaces live in the domain layer, while EF Core implementations live in the persistence layer.

Current example:

- `IUserRepository`: repository contract.
- `UserRepository`: EF Core implementation.

This keeps application use cases from depending directly on EF Core queries.

## Value Object Pattern

Value objects represent validated concepts that should not be treated as plain strings everywhere.

Examples include:

- email
- password
- refresh token

This improves validation, encapsulation, and domain readability.

## Dependency Injection

Each layer registers its services through dependency injection extension methods. Runtime wiring is handled by ASP.NET Core's built-in DI container.

## Options and Configuration Pattern

Runtime settings such as JWT issuer, audience, secret key, and logging behavior are read from configuration files such as `appsettings.json` and environment-specific settings.

## Code First Database Design

EF Core migrations define and evolve the database schema from the application model.

## Structured Logging Pattern

Serilog is used for structured logs so log events can carry named properties, not just plain text messages.

