# Database Schema

## Overview

MyEnglish API uses **SQL Server** with **Entity Framework Core** Code First approach. The database schema is designed following Domain-Driven Design principles.

## Connection String

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost,1433;Database=MyEnglishDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

## Database Tables

### Users Table

Stores user account information.

**Table Name**: `Users`

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uniqueidentifier | No | Primary key (GUID) |
| Email | nvarchar(256) | No | User email (unique) |
| Password | nvarchar(500) | No | BCrypt hashed password |
| FirstName | nvarchar(50) | No | User's first name |
| LastName | nvarchar(50) | No | User's last name |
| Gender | nvarchar(10) | No | Male, Female, or Other |
| DateOfBirth | date | Yes | User's date of birth |
| IsEmailVerified | bit | No | Email verification status |
| IsActive | bit | No | Account active status |
| LastLoginAt | datetime2 | Yes | Last login timestamp |
| CreatedDate | datetimeoffset | No | Record creation timestamp |
| LastModifiedDate | datetimeoffset | Yes | Last modification timestamp |

**Indexes**:
- `PK_Users`: Primary key on Id
- `IX_Users_Email`: Unique index on Email

**Constraints**:
- Email must be unique
- IsEmailVerified defaults to false
- IsActive defaults to true

**Entity Configuration**:
```csharp
builder.ToTable("Users");
builder.HasKey(u => u.Id);

builder.Property(u => u.Email)
    .HasConversion(
        email => email.Value,
        value => Email.Create(value))
    .HasMaxLength(256)
    .IsRequired();

builder.HasIndex(u => u.Email)
    .IsUnique()
    .HasDatabaseName("IX_Users_Email");

builder.Property(u => u.Password)
    .HasConversion(
        password => password.HashedValue,
        value => Password.CreateFromHash(value))
    .HasMaxLength(500)
    .IsRequired();
```

---

### UserRefreshTokens Table

Stores refresh tokens for user authentication.

**Table Name**: `UserRefreshTokens`

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | uniqueidentifier | No | Primary key (GUID) |
| UserId | uniqueidentifier | No | Foreign key to Users |
| Token | nvarchar(max) | No | Base64-encoded refresh token |
| ExpiresAt | datetime2 | No | Token expiration timestamp |
| IsRevoked | bit | No | Token revocation status |
| RevokedAt | datetime2 | Yes | Revocation timestamp |
| CreatedDate | datetimeoffset | No | Record creation timestamp |
| LastModifiedDate | datetimeoffset | Yes | Last modification timestamp |

**Indexes**:
- `PK_UserRefreshTokens`: Primary key on Id
- `IX_UserRefreshTokens_UserId`: Index on UserId
- `IX_UserRefreshTokens_Token`: Index on Token (for fast lookups)

**Foreign Keys**:
- `FK_UserRefreshTokens_Users_UserId`: References Users(Id) with CASCADE delete

**Entity Configuration**:
```csharp
builder.ToTable("UserRefreshTokens");
builder.HasKey(rt => rt.Id);

builder.Property(rt => rt.Token)
    .IsRequired();

builder.Property(rt => rt.IsRevoked)
    .IsRequired()
    .HasDefaultValue(false);

builder.HasOne(rt => rt.User)
    .WithMany(u => u.RefreshTokens)
    .HasForeignKey(rt => rt.UserId)
    .OnDelete(DeleteBehavior.Cascade);
```

---

## Entity Relationships

### User → UserRefreshTokens (One-to-Many)

- One User can have multiple RefreshTokens
- Cascade delete: Deleting a User deletes all their RefreshTokens
- Navigation property: `User.RefreshTokens`
- Foreign key: `UserRefreshToken.UserId`

```
Users (1) ──────< (Many) UserRefreshTokens
  Id                      UserId (FK)
```

---

## Value Objects

Value objects are stored as part of their parent entity using EF Core value conversions.

### Email Value Object

**Storage**: Stored as string in Users.Email column

**Conversion**:
```csharp
builder.Property(u => u.Email)
    .HasConversion(
        email => email.Value,        // To database
        value => Email.Create(value) // From database
    );
```

**Validation**:
- Not null or empty
- Valid email format (regex)
- Converted to lowercase

### Password Value Object

**Storage**: Stored as string in Users.Password column

**Conversion**:
```csharp
builder.Property(u => u.Password)
    .HasConversion(
        password => password.HashedValue,      // To database
        value => Password.CreateFromHash(value) // From database
    );
```

**Security**:
- Always hashed using BCrypt
- Never stored in plain text
- Includes salt automatically

### RefreshToken Value Object

**Storage**: Stored as string in UserRefreshTokens.Token column

**Properties**:
- Token: Base64-encoded random bytes
- ExpiresAt: Expiration timestamp
- IsExpired: Computed property (not stored)

---

## Migrations

### Creating Migrations

```bash
# From solution root
dotnet ef migrations add MigrationName --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api

# From Persistence project
cd server/MyEnglish.Persistence
dotnet ef migrations add MigrationName --startup-project ../MyEnglish.Api
```

### Applying Migrations

```bash
# Update to latest migration
dotnet ef database update --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api

# Update to specific migration
dotnet ef database update MigrationName --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api

# Rollback to previous migration
dotnet ef database update PreviousMigrationName --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
```

### Removing Migrations

```bash
# Remove last migration (if not applied)
dotnet ef migrations remove --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
```

### Viewing Migrations

```bash
# List all migrations
dotnet ef migrations list --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
```

---

## Database Context

**Location**: `server/MyEnglish.Persistence/DbContexts/SqlServerDbContext.cs`

```csharp
public class SqlServerDbContext : DbContext
{
    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

---

## Sample Queries

### Get User by Email

```csharp
var user = await _context.Users
    .Include(u => u.RefreshTokens)
    .FirstOrDefaultAsync(u => u.Email == email);
```

### Get User with Active Refresh Tokens

```csharp
var user = await _context.Users
    .Include(u => u.RefreshTokens.Where(rt => !rt.IsRevoked && !rt.IsExpired))
    .FirstOrDefaultAsync(u => u.Id == userId);
```

### Find User by Refresh Token

```csharp
var user = await _context.Users
    .Include(u => u.RefreshTokens)
    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
```

### Revoke All User Tokens

```csharp
var user = await _context.Users
    .Include(u => u.RefreshTokens)
    .FirstOrDefaultAsync(u => u.Id == userId);

foreach (var token in user.RefreshTokens.Where(rt => !rt.IsRevoked))
{
    token.Revoke();
}

await _context.SaveChangesAsync();
```

---

## Database Seeding

Currently, no seed data is configured. To add seed data:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    // Seed data
    modelBuilder.Entity<User>().HasData(
        new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@myenglish.com",
            // ... other properties
        }
    );
}
```

---

## Performance Considerations

### Indexes

Current indexes:
- `IX_Users_Email`: Unique index for fast email lookups
- `IX_UserRefreshTokens_UserId`: Index for user's tokens
- `IX_UserRefreshTokens_Token`: Index for token lookups

Consider adding:
- Index on `Users.IsActive` if filtering by active users frequently
- Index on `UserRefreshTokens.ExpiresAt` for cleanup queries

### Query Optimization

1. **Use Include for related data**:
   ```csharp
   var user = await _context.Users
       .Include(u => u.RefreshTokens)
       .FirstOrDefaultAsync(u => u.Id == userId);
   ```

2. **Use AsNoTracking for read-only queries**:
   ```csharp
   var users = await _context.Users
       .AsNoTracking()
       .ToListAsync();
   ```

3. **Project to DTOs to reduce data transfer**:
   ```csharp
   var userDtos = await _context.Users
       .Select(u => new UserDto
       {
           Id = u.Id,
           Email = u.Email.Value,
           // ... other properties
       })
       .ToListAsync();
   ```

---

## Backup and Restore

### Backup Database

```sql
BACKUP DATABASE MyEnglishDB
TO DISK = 'C:\Backups\MyEnglishDB.bak'
WITH FORMAT, MEDIANAME = 'MyEnglishBackup';
```

### Restore Database

```sql
RESTORE DATABASE MyEnglishDB
FROM DISK = 'C:\Backups\MyEnglishDB.bak'
WITH REPLACE;
```

---

## Maintenance

### Clean Up Expired Tokens

```sql
DELETE FROM UserRefreshTokens
WHERE ExpiresAt < GETUTCDATE();
```

Or using EF Core:

```csharp
var expiredTokens = await _context.UserRefreshTokens
    .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
    .ToListAsync();

_context.UserRefreshTokens.RemoveRange(expiredTokens);
await _context.SaveChangesAsync();
```

### Database Statistics

```sql
-- Table row counts
SELECT 
    t.NAME AS TableName,
    p.rows AS RowCount
FROM 
    sys.tables t
INNER JOIN      
    sys.partitions p ON t.object_id = p.OBJECT_ID
WHERE 
    t.is_ms_shipped = 0
    AND p.index_id IN (0,1)
ORDER BY 
    p.rows DESC;

-- Database size
EXEC sp_spaceused;
```

---

## Troubleshooting

### Migration Fails

**Problem**: Migration fails to apply

**Solutions**:
1. Check database connection
2. Verify user has CREATE/ALTER permissions
3. Review migration code for errors
4. Check for conflicting migrations

### Connection Issues

**Problem**: Cannot connect to database

**Solutions**:
1. Verify SQL Server is running
2. Check connection string
3. Verify firewall settings
4. Test connection with SSMS

### Performance Issues

**Problem**: Slow queries

**Solutions**:
1. Add appropriate indexes
2. Use AsNoTracking for read-only queries
3. Optimize Include statements
4. Review execution plans
5. Consider pagination for large datasets

---

## Future Schema Changes

Planned additions:
1. **Roles and Permissions**: Role-based access control
2. **Email Verification**: Email verification tokens
3. **Password Reset**: Password reset tokens
4. **User Profiles**: Extended profile information
5. **Audit Logs**: Track user actions
6. **Learning Progress**: Track learning activities
7. **Vocabulary**: User vocabulary lists
8. **Lessons**: Learning content
