# Development Guidelines

## Code Style and Conventions

### Naming Conventions

#### C# Naming Standards

- **Classes, Interfaces, Methods**: PascalCase
  ```csharp
  public class UserService { }
  public interface IUserRepository { }
  public void GetUserById() { }
  ```

- **Private Fields**: _camelCase with underscore prefix
  ```csharp
  private readonly IUserRepository _userRepository;
  ```

- **Parameters, Local Variables**: camelCase
  ```csharp
  public void ProcessUser(string userId, bool isActive) { }
  ```

- **Constants**: PascalCase
  ```csharp
  public const int MaxRetryAttempts = 3;
  ```

- **Interfaces**: Prefix with 'I'
  ```csharp
  public interface IUserService { }
  ```

### File Organization

- One class per file
- File name matches class name
- Organize using statements alphabetically
- Remove unused using statements

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using MyEnglish.Domain.Entities;

namespace MyEnglish.Application.Features.Users;

public class UserService
{
    // Implementation
}
```

## Architecture Guidelines

### Layer Responsibilities

#### 1. Domain Layer
- Contains business logic and rules
- No dependencies on other layers
- Defines interfaces for repositories and services
- Immutable value objects
- Domain events

**Do**:
```csharp
public class User : EntityAuditBase
{
    public Email Email { get; private set; }
    
    public void UpdateProfile(string firstName, string lastName)
    {
        // Validation and business logic
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");
            
        FirstName = firstName;
        LastName = lastName;
    }
}
```

**Don't**:
```csharp
// Don't expose setters
public Email Email { get; set; } // ❌

// Don't put data access in domain
public void Save() // ❌
{
    _dbContext.SaveChanges();
}
```

#### 2. Application Layer
- Orchestrates domain objects
- Implements use cases (Commands/Queries)
- Contains DTOs for data transfer
- Validation logic
- No direct database access

**Do**:
```csharp
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        // Orchestrate domain objects
        var user = User.Create(request.Email, request.Password, ...);
        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);
        
        return user.ToDto();
    }
}
```

#### 3. Infrastructure Layer
- Implements interfaces defined in Domain
- External service integrations
- JWT token generation
- Email services, etc.

#### 4. Persistence Layer
- Entity Framework Core configurations
- Repository implementations
- Database migrations
- Only this layer knows about EF Core

#### 5. API Layer
- FastEndpoints
- Request/Response DTOs
- Middleware
- Swagger configuration

### CQRS Pattern

#### Commands (Write Operations)

```csharp
// Command
public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth) : IRequest<AuthResponse>;

// Handler
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        // Implementation
    }
}
```

#### Queries (Read Operations)

```csharp
// Query
public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;

// Handler
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        // Implementation
    }
}
```

### Validation

Use FluentValidation for request validation:

```csharp
public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
            
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
```

### Mapping

Use manual mapping extensions instead of AutoMapper:

```csharp
public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Gender.ToString(),
            user.DateOfBirth,
            user.Age,
            user.IsEmailVerified,
            user.IsActive,
            user.LastLoginAt);
    }
}
```

## Best Practices

### Dependency Injection

- Register services in appropriate DependencyInjection classes
- Use constructor injection
- Prefer interfaces over concrete types

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
```

### Async/Await

- Use async/await for I/O operations
- Always pass CancellationToken
- Suffix async methods with 'Async'

```csharp
public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await _context.Users
        .Include(u => u.RefreshTokens)
        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
}
```

### Exception Handling

- Use specific exception types
- Let GlobalExceptionMiddleware handle exceptions
- Don't catch exceptions unless you can handle them

```csharp
// Good
if (user == null)
    throw new UnauthorizedAccessException("Invalid credentials");

// Bad
try
{
    // operation
}
catch (Exception ex)
{
    // Log and swallow - don't do this
}
```

### Logging

Use structured logging with Serilog:

```csharp
_logger.LogInformation("User {UserId} logged in successfully", userId);
_logger.LogWarning("Failed login attempt for email {Email}", email);
_logger.LogError(ex, "Error processing user registration for {Email}", email);
```

### Security

#### Password Handling
- Always hash passwords using BCrypt
- Never log passwords
- Use strong password requirements

```csharp
public static Password CreateFromPlainText(string plainTextPassword)
{
    if (plainTextPassword.Length < 6)
        throw new ArgumentException("Password must be at least 6 characters");
        
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
    return new Password(hashedPassword);
}
```

#### JWT Tokens
- Use strong secret keys (minimum 32 characters)
- Set appropriate expiration times
- Validate tokens on every request
- Store refresh tokens securely

#### Input Validation
- Validate all user inputs
- Use FluentValidation
- Sanitize inputs when necessary

### Testing Guidelines

#### Unit Tests
- Test business logic in Domain layer
- Test handlers in Application layer
- Use mocking for dependencies
- Follow AAA pattern (Arrange, Act, Assert)

```csharp
[Fact]
public void User_Create_ShouldThrowException_WhenEmailIsInvalid()
{
    // Arrange
    var invalidEmail = "not-an-email";
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => 
        User.Create(invalidEmail, "password", "John", "Doe", Genders.Male));
}
```

#### Integration Tests
- Test API endpoints
- Use in-memory database or test database
- Test authentication flows
- Test validation

### Database Guidelines

#### Entity Configuration
- Use Fluent API for configuration
- Don't use data annotations
- Configure in separate configuration classes

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasMaxLength(256)
            .IsRequired();
    }
}
```

#### Migrations
- Create migrations for schema changes
- Review migrations before applying
- Never modify applied migrations
- Use meaningful migration names

```bash
dotnet ef migrations add AddUserRefreshTokens --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
```

### Performance

#### Database Queries
- Use async operations
- Include related entities when needed
- Use pagination for large datasets
- Avoid N+1 queries

```csharp
// Good - Single query with Include
var user = await _context.Users
    .Include(u => u.RefreshTokens)
    .FirstOrDefaultAsync(u => u.Id == id);

// Bad - N+1 query
var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
var tokens = await _context.RefreshTokens.Where(t => t.UserId == id).ToListAsync();
```

#### Caching
- Cache frequently accessed data
- Use distributed cache for scalability
- Set appropriate expiration times

### Git Workflow

#### Commit Messages
- Use clear, descriptive messages
- Start with verb in present tense
- Reference issue numbers when applicable

```
Good:
- Add user registration endpoint
- Fix password validation bug
- Update JWT token expiration time

Bad:
- Fixed stuff
- WIP
- asdfasdf
```

#### Branching Strategy
- `main`: Production-ready code
- `develop`: Integration branch
- `feature/*`: New features
- `bugfix/*`: Bug fixes
- `hotfix/*`: Production hotfixes

#### Pull Requests
- Keep PRs small and focused
- Write clear descriptions
- Request reviews from team members
- Ensure all tests pass
- Update documentation

## Code Review Checklist

- [ ] Code follows naming conventions
- [ ] No unused using statements
- [ ] Proper error handling
- [ ] Logging added where appropriate
- [ ] Validation implemented
- [ ] Security considerations addressed
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No hardcoded values
- [ ] Async/await used correctly
- [ ] CancellationToken passed through
- [ ] Database queries optimized
- [ ] No code duplication

## Tools and Extensions

### Recommended Visual Studio Extensions
- ReSharper or CodeMaid
- SonarLint
- GitLens
- REST Client

### Recommended VS Code Extensions
- C# Dev Kit
- C#
- REST Client
- GitLens
- SonarLint

### Code Analysis
- Enable nullable reference types
- Use code analyzers
- Fix all warnings
- Run code cleanup before committing

## Resources

- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Clean Code Principles](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [Domain-Driven Design](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
