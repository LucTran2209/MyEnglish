# Authentication & Authorization

## Overview

MyEnglish API uses **JWT (JSON Web Token)** based authentication with refresh token mechanism for secure user authentication and authorization.

## Authentication Flow

### 1. User Registration

```
User → POST /api/auth/register → API
                                   ↓
                            Create User Account
                                   ↓
                            Generate JWT Tokens
                                   ↓
                            Return Tokens + User Info
```

**Response**:
- Access Token (JWT)
- Refresh Token
- Token Expiration Time
- User Information

### 2. User Login

```
User → POST /api/auth/login → API
                                ↓
                         Validate Credentials
                                ↓
                         Generate JWT Tokens
                                ↓
                         Return Tokens + User Info
```

### 3. Accessing Protected Resources

```
User → GET /api/users/me → API
       (with Access Token)    ↓
                        Validate Token
                              ↓
                        Return User Data
```

### 4. Token Refresh

```
User → POST /api/auth/refresh → API
       (with Refresh Token)      ↓
                           Validate Refresh Token
                                  ↓
                           Generate New Tokens
                                  ↓
                           Return New Tokens
```

### 5. Logout

```
User → POST /api/auth/logout → API
       (with Tokens)             ↓
                           Revoke Refresh Token(s)
                                 ↓
                           Return Success
```

## JWT Token Structure

### Access Token

**Purpose**: Short-lived token for API authentication

**Lifetime**: 15 minutes (configurable)

**Claims**:
```json
{
  "user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "sub": "user@example.com",
  "jti": "unique-token-id",
  "iat": 1714824000,
  "exp": 1714824900,
  "iss": "MyEnglish.Api",
  "aud": "MyEnglish.Client"
}
```

**Token Format**:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiM2ZhODVmNjQtNTcxNy00NTYyLWIzZmMtMmM5NjNmNjZhZmE2IiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29tIn0.signature
```

### Refresh Token

**Purpose**: Long-lived token for obtaining new access tokens

**Lifetime**: 7 days (configurable)

**Storage**: Database (UserRefreshToken table)

**Format**: Base64-encoded random bytes (32 bytes)

**Properties**:
- Token string
- Expiration date
- Revoked status
- User association

## Configuration

### JWT Settings (appsettings.json)

```json
{
  "Jwt": {
    "SecretKey": "YourSecretKeyHere-MustBeAtLeast32CharactersLong!",
    "Issuer": "MyEnglish.Api",
    "Audience": "MyEnglish.Client",
    "AccessTokenExpirationMinutes": 15
  }
}
```

**Important**: 
- `SecretKey` must be at least 32 characters
- Use a strong, random secret key in production
- Store secret key in environment variables or Azure Key Vault
- Never commit secret keys to source control

### Refresh Token Settings

Configured in `RefreshToken.Generate()` method:

```csharp
public static RefreshToken Generate(int expirationDays = 7)
{
    // Generates secure random token
    // Default expiration: 7 days
}
```

## Implementation Details

### JWT Token Generation

Located in: `MyEnglish.Infrastructure/Services/JwtTokenService.cs`

```csharp
public string GenerateAccessToken(User user)
{
    var claims = new[]
    {
        new Claim("user_id", user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email.Value),
        new Claim(ClaimTypes.NameIdentifier, user.Email.Value),
        new Claim(JwtRegisteredClaimNames.Sub, user.Email.Value),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.Add(AccessTokenExpiration),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### Refresh Token Generation

Located in: `MyEnglish.Domain/ValueObjects/RefreshToken.cs`

```csharp
public static RefreshToken Generate(int expirationDays = 7)
{
    var randomBytes = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);
    
    var token = Convert.ToBase64String(randomBytes);
    var expiresAt = DateTime.UtcNow.AddDays(expirationDays);
    
    return new RefreshToken(token, expiresAt);
}
```

### Token Validation

Configured in: `MyEnglish.Infrastructure/DependencyInjections/DependencyInjection.cs`

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
```

## Using Authentication in Endpoints

### Protecting Endpoints

```csharp
public class GetCurrentUserEndpoint : Endpoint<EmptyRequest, UserDto>
{
    public override void Configure()
    {
        Get("/api/users/me");
        Roles(); // Requires authentication
    }
    
    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        // Get user ID from claims
        var userIdClaim = User.FindFirst("user_id")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        // Use userId to fetch user data
    }
}
```

### Allowing Anonymous Access

```csharp
public class LoginEndpoint : Endpoint<LoginRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous(); // No authentication required
    }
}
```

## Security Best Practices

### Token Security

1. **Use HTTPS**: Always use HTTPS in production
2. **Short-lived Access Tokens**: Keep access token lifetime short (15 minutes)
3. **Secure Refresh Tokens**: Store refresh tokens securely in database
4. **Token Rotation**: Generate new refresh token on each refresh
5. **Revocation**: Implement token revocation for logout

### Password Security

1. **Hashing**: Use BCrypt for password hashing
2. **Salt**: BCrypt automatically handles salting
3. **Work Factor**: BCrypt default work factor is secure
4. **Never Store Plain Text**: Never store passwords in plain text

```csharp
// Password hashing
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);

// Password verification
var isValid = BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
```

### Secret Key Management

**Development**:
```json
{
  "Jwt": {
    "SecretKey": "Development-Secret-Key-32-Characters-Long!"
  }
}
```

**Production** (use environment variables):
```bash
export JWT__SECRETKEY="Production-Secret-Key-From-Secure-Storage"
```

Or use Azure Key Vault, AWS Secrets Manager, etc.

### CORS Configuration

Configure CORS appropriately:

```csharp
// Development - Allow all
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Production - Restrict origins
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```

## Common Scenarios

### Scenario 1: User Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response**:
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "base64token...",
  "expiresAt": "2026-05-04T10:30:00Z",
  "user": { ... }
}
```

**Client Action**:
1. Store access token in memory or sessionStorage
2. Store refresh token in httpOnly cookie or secure storage
3. Use access token for API requests

### Scenario 2: Accessing Protected Resource

```http
GET /api/users/me
Authorization: Bearer eyJhbGci...
```

**Response**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  ...
}
```

### Scenario 3: Token Expired

```http
GET /api/users/me
Authorization: Bearer expired_token
```

**Response**:
```http
401 Unauthorized
```

**Client Action**:
1. Detect 401 error
2. Call refresh token endpoint
3. Retry original request with new token

### Scenario 4: Refresh Token

```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "base64token..."
}
```

**Response**:
```json
{
  "accessToken": "new_eyJhbGci...",
  "refreshToken": "new_base64token...",
  "expiresAt": "2026-05-04T11:00:00Z",
  "user": { ... }
}
```

### Scenario 5: Logout

```http
POST /api/auth/logout
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{
  "refreshToken": "base64token..."
}
```

**Response**:
```http
200 OK
```

**Client Action**:
1. Clear stored tokens
2. Redirect to login page

### Scenario 6: Logout from All Devices

```http
POST /api/auth/logout
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{
  "refreshToken": null
}
```

This revokes all refresh tokens for the user.

## Client-Side Implementation

### JavaScript/TypeScript Example

```typescript
class AuthService {
  private accessToken: string | null = null;
  private refreshToken: string | null = null;

  async login(email: string, password: string) {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });

    const data = await response.json();
    this.accessToken = data.accessToken;
    this.refreshToken = data.refreshToken;
    
    // Store refresh token securely
    localStorage.setItem('refreshToken', data.refreshToken);
  }

  async apiCall(url: string, options: RequestInit = {}) {
    options.headers = {
      ...options.headers,
      'Authorization': `Bearer ${this.accessToken}`
    };

    let response = await fetch(url, options);

    // If token expired, refresh and retry
    if (response.status === 401) {
      await this.refreshAccessToken();
      options.headers['Authorization'] = `Bearer ${this.accessToken}`;
      response = await fetch(url, options);
    }

    return response;
  }

  async refreshAccessToken() {
    const response = await fetch('/api/auth/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: this.refreshToken })
    });

    const data = await response.json();
    this.accessToken = data.accessToken;
    this.refreshToken = data.refreshToken;
    
    localStorage.setItem('refreshToken', data.refreshToken);
  }

  async logout() {
    await fetch('/api/auth/logout', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${this.accessToken}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ refreshToken: this.refreshToken })
    });

    this.accessToken = null;
    this.refreshToken = null;
    localStorage.removeItem('refreshToken');
  }
}
```

## Troubleshooting

### Token Validation Fails

**Symptoms**: 401 Unauthorized on all requests

**Possible Causes**:
1. Secret key mismatch
2. Token expired
3. Invalid token format
4. Clock skew issues

**Solutions**:
1. Verify secret key in configuration
2. Check token expiration time
3. Ensure token is properly formatted
4. Set `ClockSkew = TimeSpan.Zero` in token validation

### Refresh Token Not Working

**Symptoms**: Refresh endpoint returns 401

**Possible Causes**:
1. Refresh token expired
2. Refresh token revoked
3. User account deactivated
4. Token not found in database

**Solutions**:
1. Check token expiration in database
2. Verify token hasn't been revoked
3. Check user account status
4. Ensure token exists in UserRefreshTokens table

### CORS Errors

**Symptoms**: Browser blocks requests

**Solutions**:
1. Configure CORS in API
2. Include credentials in requests
3. Verify allowed origins
4. Check preflight requests

## Future Enhancements

Consider implementing:
1. **Role-Based Authorization**: Add roles and permissions
2. **Two-Factor Authentication**: Add 2FA support
3. **OAuth2/OpenID Connect**: Support external providers
4. **Token Blacklisting**: Implement token blacklist for immediate revocation
5. **Rate Limiting**: Prevent brute force attacks
6. **Account Lockout**: Lock accounts after failed attempts
7. **Email Verification**: Verify email addresses
8. **Password Reset**: Implement password reset flow
