# MyEnglish API - Project Overview

## Introduction

MyEnglish is a comprehensive English learning platform API built with modern .NET technologies and architectural patterns.

## Technology Stack

### Backend Framework
- **.NET 9.0**: Latest version of .NET
- **ASP.NET Core**: Web API framework
- **C# 12**: Programming language

### Architecture & Patterns
- **Domain-Driven Design (DDD)**: Rich domain models with encapsulation
- **CQRS Pattern**: Command Query Responsibility Segregation using MediatR
- **Clean Architecture**: Separation of concerns across layers
- **Repository Pattern**: Data access abstraction
- **Value Objects**: Email, Password, RefreshToken

### Libraries & Packages
- **FastEndpoints**: Lightweight alternative to MVC controllers
- **MediatR**: In-process messaging for CQRS
- **FluentValidation**: Request validation
- **Entity Framework Core 9.0**: ORM for database access
- **Serilog**: Structured logging to console and files
- **BCrypt.Net**: Password hashing
- **JWT Bearer Authentication**: Token-based authentication

### Database
- **SQL Server**: Primary database
- **Code First Approach**: Database schema from domain models

## Project Structure

```
MyEnglish/
├── src/
│   ├── MyEnglish.Api/              # API Layer (Endpoints, Middleware)
│   ├── MyEnglish.Application/      # Application Layer (CQRS, DTOs)
│   ├── MyEnglish.Domain/           # Domain Layer (Entities, Value Objects)
│   ├── MyEnglish.Infrastructure/   # Infrastructure (JWT, Services)
│   └── MyEnglish.Persistence/      # Data Access (EF Core, Repositories)
├── Wiki/                           # Documentation
└── Directory.Packages.props        # Centralized package management
```

## Key Features

### Authentication & Authorization
- User registration with validation
- Login with JWT access tokens
- Refresh token mechanism
- Token expiration handling
- Role-based authorization

### Domain Features
- User management with profile updates
- Email verification support
- Account activation/deactivation
- Password management with BCrypt hashing
- Refresh token management (multiple devices)

### Technical Features
- Global exception handling
- Request/response logging
- Swagger/OpenAPI documentation
- Health checks
- CORS support
- Validation pipeline

## Architecture Layers

### 1. API Layer (MyEnglish.Api)
- FastEndpoints for HTTP endpoints
- Request/Response DTOs
- Middleware (Exception handling)
- Swagger configuration

### 2. Application Layer (MyEnglish.Application)
- CQRS Commands and Queries
- Command/Query Handlers
- Application DTOs
- FluentValidation validators
- MediatR pipeline behaviors

### 3. Domain Layer (MyEnglish.Domain)
- Domain entities (User, UserRefreshToken)
- Value objects (Email, Password, RefreshToken)
- Domain events
- Repository interfaces
- Domain services interfaces

### 4. Infrastructure Layer (MyEnglish.Infrastructure)
- JWT token service implementation
- External service integrations
- Cross-cutting concerns

### 5. Persistence Layer (MyEnglish.Persistence)
- Entity Framework Core DbContext
- Entity configurations
- Repository implementations
- Database migrations

## Design Principles

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes are substitutable
- **Interface Segregation**: Specific interfaces over general ones
- **Dependency Inversion**: Depend on abstractions, not concretions

### DDD Principles
- **Ubiquitous Language**: Consistent terminology
- **Bounded Contexts**: Clear boundaries
- **Aggregates**: Consistency boundaries
- **Value Objects**: Immutable, validated objects
- **Domain Events**: Capture domain occurrences

## Getting Started

See [02-Getting-Started.md](02-Getting-Started.md) for setup instructions.

## API Documentation

See [03-API-Endpoints.md](03-API-Endpoints.md) for endpoint documentation.

## Development Guidelines

See [04-Development-Guidelines.md](04-Development-Guidelines.md) for coding standards.
