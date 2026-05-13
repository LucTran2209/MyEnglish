# Logging Documentation

## Overview

MyEnglish API uses **Serilog** for structured logging to both console and file outputs. Logs provide detailed information about application behavior, errors, and performance.

## Log Configuration

### Log Levels

Logs are categorized by severity:

| Level | Description | When to Use |
|-------|-------------|-------------|
| **Debug** | Detailed diagnostic information | Development only, troubleshooting |
| **Information** | General application flow | Normal operations, key events |
| **Warning** | Potentially harmful situations | Deprecated features, misconfigurations |
| **Error** | Error events that allow app to continue | Handled exceptions, failed operations |
| **Fatal** | Severe errors causing termination | Unrecoverable errors, startup failures |

### Environment-Specific Configuration

#### Development (appsettings.Development.json)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Warning"
      }
    }
  }
}
```

#### Production (appsettings.json)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Warning"
      }
    }
  }
}
```

## Log Outputs

### Console Logging

Logs are written to the console in real-time for immediate feedback during development.

**Format**:
```
[2026-05-04 10:15:30 INF] Application started successfully
[2026-05-04 10:15:35 WRN] User not found for email: test@example.com
[2026-05-04 10:15:40 ERR] Database connection failed
```

**Template**:
```
[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}
```

### File Logging

Logs are persisted to text files for historical analysis and debugging.

**Location**: `server/MyEnglish.Api/Logs/`

**File Naming**: `myenglish-YYYYMMDD.txt`
- Example: `myenglish-20260504.txt`

**Format**:
```
[2026-05-04 10:15:30.123 +00:00] [INF] [MyEnglish.Api.Endpoints.Auth.LoginEndpoint] User logged in successfully
```

**Template**:
```
[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}
```

### Log Rotation

- **Rolling Interval**: Daily (new file created each day)
- **Retention Period**: 30 days (older files automatically deleted)
- **File Size Limit**: 10MB per file
- **Roll on Size**: Creates new file if size limit exceeded
- **Shared Mode**: Multiple processes can write safely

## What Gets Logged

### Application Lifecycle

```csharp
Log.Information("Starting MyEnglish API application");
Log.Information("MyEnglish API application started successfully");
Log.Fatal(ex, "Application terminated unexpectedly");
```

### HTTP Requests

Every HTTP request is automatically logged with:
- HTTP method (GET, POST, PUT, DELETE)
- Request path
- Status code
- Response time in milliseconds
- Request host and scheme
- User agent
- Remote IP address

**Example**:
```
[2026-05-04 10:15:30.123 +00:00] [INF] HTTP POST /api/auth/login responded 200 in 45.2345 ms
```

### Exceptions

Unhandled exceptions are logged with full stack traces:

```
[2026-05-04 10:15:30.123 +00:00] [ERR] [MyEnglish.Api.Middleware.GlobalExceptionMiddleware] An unhandled exception occurred
System.UnauthorizedAccessException: Invalid credentials
   at MyEnglish.Application.Features.Login.LoginCommandHandler.Handle(...)
   at MediatR.Pipeline.RequestExceptionProcessorBehavior`2.Handle(...)
```

### Custom Logging

You can add custom logging in your code:

```csharp
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        
        var user = await _userRepository.GetByEmailAsync(email, ct);
        
        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        
        _logger.LogInformation("User {UserId} logged in successfully", user.Id);
        
        return response;
    }
}
```

## Structured Logging

Serilog supports structured logging with properties:

```csharp
// Good - Structured
_logger.LogInformation("User {UserId} updated profile with {FirstName} {LastName}", 
    userId, firstName, lastName);

// Bad - String interpolation
_logger.LogInformation($"User {userId} updated profile with {firstName} {lastName}");
```

**Benefits**:
- Properties can be queried and filtered
- Better for log aggregation tools
- Easier to analyze and search

## Log Enrichment

Logs are automatically enriched with:
- **Application**: "MyEnglish.Api"
- **MachineName**: Server/computer name
- **ThreadId**: Thread identifier
- **SourceContext**: Class name that generated the log

## Viewing Logs

### During Development

1. **Console Output**: View real-time logs in the terminal/console
2. **Visual Studio Output Window**: View logs in the Output window
3. **Log Files**: Open files in `server/MyEnglish.Api/Logs/`

### In Production

1. **Log Files**: Access files on the server
2. **Log Aggregation**: Use tools like:
   - **Seq**: Structured log viewer
   - **ELK Stack**: Elasticsearch, Logstash, Kibana
   - **Splunk**: Enterprise log management
   - **Azure Application Insights**: Cloud-based monitoring

## Log Analysis

### Finding Errors

```bash
# Windows PowerShell
Select-String -Path "Logs\myenglish-*.txt" -Pattern "\[ERR\]"

# Linux/Mac
grep "\[ERR\]" Logs/myenglish-*.txt
```

### Finding Specific User Activity

```bash
# Search for specific user ID
Select-String -Path "Logs\myenglish-*.txt" -Pattern "UserId.*3fa85f64"
```

### Performance Analysis

```bash
# Find slow requests (>1000ms)
Select-String -Path "Logs\myenglish-*.txt" -Pattern "responded.*in [0-9]{4,}\."
```

## Best Practices

### Do's ✅

1. **Use appropriate log levels**
   ```csharp
   _logger.LogDebug("Detailed diagnostic info");
   _logger.LogInformation("Normal operation");
   _logger.LogWarning("Potential issue");
   _logger.LogError(ex, "Error occurred");
   _logger.LogCritical(ex, "Critical failure");
   ```

2. **Use structured logging**
   ```csharp
   _logger.LogInformation("User {UserId} performed {Action}", userId, action);
   ```

3. **Include context**
   ```csharp
   _logger.LogError(ex, "Failed to process payment for Order {OrderId}", orderId);
   ```

4. **Log important business events**
   ```csharp
   _logger.LogInformation("User {UserId} registered successfully", user.Id);
   _logger.LogInformation("Password changed for User {UserId}", userId);
   ```

### Don'ts ❌

1. **Don't log sensitive information**
   ```csharp
   // Bad - Logs password
   _logger.LogInformation("User login: {Email} {Password}", email, password);
   
   // Good
   _logger.LogInformation("User login attempt: {Email}", email);
   ```

2. **Don't log in loops**
   ```csharp
   // Bad
   foreach (var user in users)
   {
       _logger.LogInformation("Processing user {UserId}", user.Id);
   }
   
   // Good
   _logger.LogInformation("Processing {Count} users", users.Count);
   ```

3. **Don't use string interpolation**
   ```csharp
   // Bad
   _logger.LogInformation($"User {userId} logged in");
   
   // Good
   _logger.LogInformation("User {UserId} logged in", userId);
   ```

4. **Don't catch and log without rethrowing**
   ```csharp
   // Bad
   try
   {
       // operation
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error occurred");
       // Exception is swallowed
   }
   
   // Good - Let GlobalExceptionMiddleware handle it
   // Or rethrow if you need to log additional context
   try
   {
       // operation
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error in specific context: {Context}", context);
       throw;
   }
   ```

## Configuration Options

### Changing Log Level at Runtime

Update `appsettings.json` and restart the application:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"  // Change to Debug for more detailed logs
    }
  }
}
```

### Changing Log File Location

Update `Program.cs`:

```csharp
.WriteTo.File(
    path: "C:/Logs/myenglish-.txt",  // Custom path
    rollingInterval: RollingInterval.Day)
```

### Adding Additional Sinks

#### Seq (Structured Log Viewer)

1. Install package:
   ```bash
   dotnet add package Serilog.Sinks.Seq
   ```

2. Update `Program.cs`:
   ```csharp
   .WriteTo.Seq("http://localhost:5341")
   ```

#### Azure Application Insights

1. Install package:
   ```bash
   dotnet add package Serilog.Sinks.ApplicationInsights
   ```

2. Update `Program.cs`:
   ```csharp
   .WriteTo.ApplicationInsights(
       telemetryConfiguration,
       TelemetryConverter.Traces)
   ```

## Troubleshooting

### Logs Not Being Created

1. Check folder permissions
2. Verify path exists or can be created
3. Check disk space
4. Review console for Serilog errors

### Logs Too Verbose

1. Increase minimum log level to Warning or Error
2. Add overrides for noisy namespaces:
   ```json
   {
     "Serilog": {
       "MinimumLevel": {
         "Override": {
           "Microsoft.EntityFrameworkCore": "Warning"
         }
       }
     }
   }
   ```

### Performance Impact

- File logging has minimal performance impact
- Use async sinks for high-throughput scenarios
- Consider log level in production (Information or Warning)
- Monitor disk I/O if writing large volumes

## Log Retention and Cleanup

Logs older than 30 days are automatically deleted. To change retention:

```csharp
.WriteTo.File(
    path: "Logs/myenglish-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 60)  // Keep 60 days
```

To disable automatic cleanup:

```csharp
retainedFileCountLimit: null  // Keep all logs
```

## Security Considerations

1. **Never log**:
   - Passwords
   - API keys
   - Tokens (access or refresh)
   - Credit card numbers
   - Personal identification numbers
   - Social security numbers

2. **Be careful with**:
   - Email addresses (consider masking)
   - IP addresses (GDPR considerations)
   - User IDs (use only when necessary)

3. **Protect log files**:
   - Restrict file system permissions
   - Encrypt logs if containing sensitive data
   - Secure log aggregation endpoints
   - Implement log retention policies

## Monitoring and Alerts

Consider setting up alerts for:
- High error rates
- Fatal errors
- Slow response times
- Authentication failures
- Unusual activity patterns

Use log aggregation tools to create dashboards and alerts based on log data.
