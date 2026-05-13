# Getting Started

## Prerequisites

### Required Software
- **.NET 9.0 SDK** or later
- **SQL Server 2019** or later (or SQL Server Express)
- **Visual Studio 2022** or **Visual Studio Code** or **JetBrains Rider**
- **Git** for version control

### Optional Tools
- **Postman** or **Insomnia** for API testing
- **SQL Server Management Studio (SSMS)** for database management
- **Docker Desktop** (if using containerized SQL Server)

## Installation Steps

### 1. Clone the Repository

```bash
git clone <repository-url>
cd MyEnglish
```

### 2. Configure Database Connection

Update the connection string in `server/MyEnglish.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost,1433;Database=MyEnglishDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

**Connection String Parameters:**
- `Server`: Your SQL Server instance (localhost for local development)
- `Database`: Database name (will be created if doesn't exist)
- `User Id`: SQL Server username
- `Password`: SQL Server password
- `TrustServerCertificate=True`: Required for local development

### 3. Configure JWT Settings

Update JWT settings in `server/MyEnglish.Api/appsettings.json`:

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

**Important**: Change the `SecretKey` to a strong, unique value in production!

### 4. Restore NuGet Packages

```bash
dotnet restore
```

### 5. Apply Database Migrations

```bash
cd server/MyEnglish.Persistence
dotnet ef database update --startup-project ../MyEnglish.Api
```

Or from the solution root:

```bash
dotnet ef database update --project server/MyEnglish.Persistence --startup-project server/MyEnglish.Api
```

### 6. Build the Solution

```bash
dotnet build
```

### 7. Run the Application

```bash
cd server/MyEnglish.Api
dotnet run
```

Or use Visual Studio:
- Open `MyEnglish.sln`
- Set `MyEnglish.Api` as startup project
- Press F5 to run

## Verify Installation

### 1. Check Application is Running

The application should start on:
- **HTTPS**: `https://localhost:7001`
- **HTTP**: `http://localhost:5000`

### 2. Access Swagger UI

Open your browser and navigate to:
```
https://localhost:7001/swagger
```

You should see the Swagger UI with all available endpoints.

### 3. Check Health Endpoint

```bash
curl https://localhost:7001/api/health
```

### 4. Test Registration Endpoint

```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "confirmPassword": "Test123!",
    "firstName": "John",
    "lastName": "Doe",
    "gender": "Male",
    "dateOfBirth": "1990-01-01"
  }'
```

## Development Environment Setup

### Visual Studio 2022

1. Open `MyEnglish.sln`
2. Set `MyEnglish.Api` as startup project
3. Configure launch settings in `Properties/launchSettings.json`
4. Press F5 to run with debugging

### Visual Studio Code

1. Open the project folder
2. Install recommended extensions:
   - C# Dev Kit
   - C#
   - REST Client (for testing APIs)
3. Use the integrated terminal to run `dotnet run`
4. Use F5 for debugging (requires launch.json configuration)

### JetBrains Rider

1. Open `MyEnglish.sln`
2. Rider will automatically detect the startup project
3. Configure run configurations if needed
4. Press Shift+F10 to run or Shift+F9 to debug

## Docker Setup (Optional)

### Using SQL Server in Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Using Docker Compose

Create `docker-compose.yml`:

```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql

volumes:
  sqlserver-data:
```

Run:
```bash
docker-compose up -d
```

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to SQL Server

**Solutions**:
1. Verify SQL Server is running
2. Check connection string is correct
3. Ensure SQL Server allows remote connections
4. Check firewall settings
5. Verify SQL Server authentication mode (Mixed Mode)

### Migration Issues

**Problem**: `dotnet ef` command not found

**Solution**:
```bash
dotnet tool install --global dotnet-ef
```

**Problem**: Migration fails

**Solutions**:
1. Ensure database server is running
2. Check connection string
3. Verify user has CREATE DATABASE permissions
4. Delete existing database and retry

### Build Errors

**Problem**: Package restore fails

**Solution**:
```bash
dotnet clean
dotnet restore
dotnet build
```

**Problem**: Process is locking DLL files

**Solution**:
1. Stop all running instances of the application
2. Close Visual Studio
3. Delete `bin` and `obj` folders
4. Rebuild the solution

### Port Already in Use

**Problem**: Port 5000 or 7001 is already in use

**Solution**:
Update `server/MyEnglish.Api/Properties/launchSettings.json`:

```json
{
  "applicationUrl": "https://localhost:7002;http://localhost:5001"
}
```

## Next Steps

- Read [03-API-Endpoints.md](03-API-Endpoints.md) to learn about available endpoints
- Read [04-Development-Guidelines.md](04-Development-Guidelines.md) for coding standards
- Read [05-Logging.md](05-Logging.md) to understand logging configuration
- Read [06-Authentication.md](06-Authentication.md) for authentication details
