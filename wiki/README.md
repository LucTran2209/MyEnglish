# MyEnglish Wiki

This folder contains documentation for the MyEnglish system. It is intentionally kept at the solution root, separate from `client` and `server`, because it documents the whole product instead of belonging to one runtime project.

## Structure

- [01-Project-Overview.md](01-Project-Overview.md): solution structure and high-level architecture.
- [02-Getting-Started.md](02-Getting-Started.md): local setup, configuration, build, and run steps.
- [03-API-Endpoints.md](03-API-Endpoints.md): backend API reference and request examples.
- [04-Development-Guidelines.md](04-Development-Guidelines.md): coding standards and development workflow.
- [05-Logging.md](05-Logging.md): Serilog configuration, log format, and troubleshooting.
- [06-Authentication.md](06-Authentication.md): JWT authentication and refresh token behavior.
- [07-Database-Schema.md](07-Database-Schema.md): SQL Server schema, migrations, and database notes.
- [08-Technologies.md](08-Technologies.md): frameworks, libraries, and tools used by the system.
- [09-Architecture-Patterns.md](09-Architecture-Patterns.md): architecture and design patterns used in the codebase.
- [10-Engineering-Techniques.md](10-Engineering-Techniques.md): implementation techniques and engineering practices.

## Documentation Rules

- Keep system-wide documentation in `wiki`.
- Keep frontend-specific notes under `client/MyEnglish.Web` only when they are local implementation notes.
- Keep backend-specific notes under `server` only when they are code comments or generated artifacts.
- Add one topic per Markdown file when the topic is large enough to evolve independently.

