# API Endpoints Documentation

## Base URL

- **Development**: `https://localhost:7001/api`
- **Production**: `https://your-domain.com/api`

## Authentication Endpoints

### Register User

Creates a new user account.

**Endpoint**: `POST /api/auth/register`

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "gender": "Male",
  "dateOfBirth": "1990-01-01"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresAt": "2026-05-04T10:30:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "gender": "Male",
    "dateOfBirth": "1990-01-01",
    "age": 36,
    "isEmailVerified": false,
    "isActive": true,
    "lastLoginAt": null
  }
}
```

**Validation Rules**:
- Email: Required, valid email format
- Password: Required, minimum 6 characters
- ConfirmPassword: Must match password
- FirstName: Required, max 50 characters
- LastName: Required, max 50 characters
- Gender: Required (Male, Female, Other)
- DateOfBirth: Optional, cannot be in the future

---

### Login

Authenticates a user and returns access and refresh tokens.

**Endpoint**: `POST /api/auth/login`

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresAt": "2026-05-04T10:30:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "gender": "Male",
    "dateOfBirth": "1990-01-01",
    "age": 36,
    "isEmailVerified": false,
    "isActive": true,
    "lastLoginAt": "2026-05-04T10:15:00Z"
  }
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid email or password
- `401 Unauthorized`: User account is deactivated

---

### Refresh Token

Generates new access and refresh tokens using a valid refresh token.

**Endpoint**: `POST /api/auth/refresh`

**Request Body**:
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "new-base64-encoded-refresh-token",
  "expiresAt": "2026-05-04T10:45:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "gender": "Male",
    "dateOfBirth": "1990-01-01",
    "age": 36,
    "isEmailVerified": false,
    "isActive": true,
    "lastLoginAt": "2026-05-04T10:15:00Z"
  }
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or expired refresh token
- `401 Unauthorized`: User account is deactivated

---

### Logout

Revokes refresh tokens. Can logout from current device or all devices.

**Endpoint**: `POST /api/auth/logout`

**Authentication**: Required (Bearer token)

**Request Body**:
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Note**: If `refreshToken` is null or empty, all refresh tokens for the user will be revoked (logout from all devices).

**Response** (200 OK):
```json
{}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing access token

---

## User Endpoints

### Get Current User

Retrieves the authenticated user's profile information.

**Endpoint**: `GET /api/users/me`

**Authentication**: Required (Bearer token)

**Response** (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "fullName": "John Doe",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "age": 36,
  "isEmailVerified": false,
  "isActive": true,
  "lastLoginAt": "2026-05-04T10:15:00Z"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing access token
- `404 Not Found`: User not found

---

### Update User Profile

Updates the authenticated user's profile information.

**Endpoint**: `PUT /api/users/me`

**Authentication**: Required (Bearer token)

**Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "gender": "Male",
  "dateOfBirth": "1990-01-01"
}
```

**Response** (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Smith",
  "fullName": "John Smith",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "age": 36,
  "isEmailVerified": false,
  "isActive": true,
  "lastLoginAt": "2026-05-04T10:15:00Z"
}
```

**Validation Rules**:
- FirstName: Required, max 50 characters
- LastName: Required, max 50 characters
- Gender: Required (Male, Female, Other)
- DateOfBirth: Optional, cannot be in the future

**Error Responses**:
- `401 Unauthorized`: Invalid or missing access token
- `400 Bad Request`: Validation errors

---

## Authentication Header

For protected endpoints, include the JWT access token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Error Response Format

All error responses follow this format:

```json
{
  "message": "Error description",
  "details": [
    "Detailed error message 1",
    "Detailed error message 2"
  ]
}
```

## HTTP Status Codes

- `200 OK`: Request succeeded
- `400 Bad Request`: Validation error or invalid request
- `401 Unauthorized`: Authentication required or failed
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Rate Limiting

Currently, no rate limiting is implemented. Consider adding rate limiting in production.

## CORS

CORS is configured to allow requests from any origin in development. Update CORS policy for production.

## Swagger/OpenAPI

Interactive API documentation is available at:
```
https://localhost:7001/swagger
```

Use Swagger UI to:
- Explore all endpoints
- Test API calls
- View request/response schemas
- Authenticate with JWT tokens

## Postman Collection

You can import the API into Postman:

1. Open Postman
2. Import → Link
3. Enter: `https://localhost:7001/swagger/v1/swagger.json`

Or create a collection manually with the endpoints above.

## Example Workflow

### 1. Register a New User

```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe",
    "gender": "Male",
    "dateOfBirth": "1990-01-01"
  }'
```

### 2. Login

```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

Save the `accessToken` and `refreshToken` from the response.

### 3. Get Current User

```bash
curl -X GET https://localhost:7001/api/users/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 4. Update Profile

```bash
curl -X PUT https://localhost:7001/api/users/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Smith",
    "gender": "Male",
    "dateOfBirth": "1990-01-01"
  }'
```

### 5. Refresh Token

```bash
curl -X POST https://localhost:7001/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "YOUR_REFRESH_TOKEN"
  }'
```

### 6. Logout

```bash
curl -X POST https://localhost:7001/api/auth/logout \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "YOUR_REFRESH_TOKEN"
  }'
```
