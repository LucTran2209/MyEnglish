# Technologies

This file lists the main technologies, frameworks, libraries, and tools used by MyEnglish.

## Backend

- **.NET 9**: backend runtime and application framework.
- **ASP.NET Core Web API**: HTTP API hosting.
- **C#**: backend programming language.
- **FastEndpoints**: endpoint-based API implementation without MVC controllers.
- **Swagger/OpenAPI**: API exploration and documentation.

## Backend Application Libraries

- **MediatR**: in-process messaging for command/query dispatch.
- **FluentValidation**: request and command validation.
- **JWT Bearer Authentication**: stateless access token authentication.
- **System.IdentityModel.Tokens.Jwt**: JWT creation and validation support.
- **BCrypt.Net-Next**: password hashing.

## Backend Data Layer

- **Entity Framework Core 9**: ORM and data access.
- **Entity Framework Core SQL Server Provider**: SQL Server integration.
- **Entity Framework Core Tools**: migrations and schema updates.
- **SQL Server**: primary relational database.

## Logging and Observability

- **Serilog.AspNetCore**: structured logging integration for ASP.NET Core.
- **Serilog.Sinks.Console**: console log output.
- **Serilog.Sinks.File**: file log output and rolling log files.

## Frontend

- **React 19**: frontend UI library.
- **React DOM**: browser rendering package for React.
- **TypeScript**: type-safe frontend language.
- **Vite**: frontend dev server, bundler, and production build tool.
- **@vitejs/plugin-react**: React support for Vite.

## Tooling

- **Visual Studio Solution (`MyEnglish.sln`)**: backend project organization.
- **Central Package Management (`Directory.Packages.props`)**: shared NuGet package versions.
- **npm**: frontend package management.
- **Dockerfile**: container build support for the API project.

